(function() {
    var e = window.AmCharts;
    e.AmMap = e.Class({
        inherits: e.AmChart,
        construct: function(a) {
            this.cname = "AmMap";
            this.type = "map";
            this.theme = a;
            this.svgNotSupported = "This browser doesn't support SVG. Use Chrome, Firefox, Internet Explorer 9 or later.";
            this.createEvents("rollOverMapObject", "rollOutMapObject", "clickMapObject", "mouseDownMapObject", "selectedObjectChanged", "homeButtonClicked", "zoomCompleted", "dragCompleted", "positionChanged", "writeDevInfo", "click");
            this.zoomDuration = 1;
            this.zoomControl = new e.ZoomControl(a);
            this.fitMapToContainer = !0;
            this.mouseWheelZoomEnabled = this.backgroundZoomsToTop = !1;
            this.allowClickOnSelectedObject = this.useHandCursorOnClickableOjects = this.showBalloonOnSelectedObject = !0;
            this.showObjectsAfterZoom = this.wheelBusy = !1;
            this.zoomOnDoubleClick = this.useObjectColorForBalloon = !0;
            this.allowMultipleDescriptionWindows = !1;
            this.dragMap = this.centerMap = this.linesAboveImages = !0;
            this.colorSteps = 5;
            this.showAreasInList = !0;
            this.showLinesInList = this.showImagesInList = !1;
            this.areasProcessor = new e.AreasProcessor(this);
            this.areasSettings = new e.AreasSettings(a);
            this.imagesProcessor = new e.ImagesProcessor(this);
            this.imagesSettings = new e.ImagesSettings(a);
            this.linesProcessor = new e.LinesProcessor(this);
            this.linesSettings = new e.LinesSettings(a);
            this.initialTouchZoom = 1;
            this.showDescriptionOnHover = !1;
            e.AmMap.base.construct.call(this, a);
            this.creditsPosition = "bottom-left";
            this.product = "ammap";
            this.areasClasses = {};
            e.applyTheme(this, a, this.cname)
        },
        initChart: function() {
            this.zoomInstantly = !0;
            var a = this.container;
            this.panRequired = !0;
            if (this.sizeChanged && e.hasSVG && this.chartCreated) {
                this.freeLabelsSet && this.freeLabelsSet.remove();
                this.freeLabelsSet = a.set();
                this.container.setSize(this.realWidth, this.realHeight);
                this.resizeMap();
                this.drawBackground();
                this.redrawLabels();
                this.drawTitles();
                this.processObjects();
                this.rescaleObjects();
                this.zoomControl.init(this, a);
                this.drawBg();
                var b = this.smallMap;
                b && b.init(this, a);
                (b = this.valueLegend) && b.init(this, a);
                this.sizeChanged = !1;
                this.zoomToLongLat(this.zLevelTemp, this.zLongTemp, this.zLatTemp, !0);
                this.previousWidth = this.realWidth;
                this.previousHeight = this.realHeight;
                this.updateSmallMap();
                this.linkSet.toFront();
                this.zoomControl.update && this.zoomControl.update()
            } else(e.AmMap.base.initChart.call(this), e.hasSVG) ? (this.dataChanged && (this.parseData(), this.dispatchDataUpdated = !0, this.dataChanged = !1, a = this.legend) && (a.position = "absolute", a.invalidateSize()), this.createDescriptionsDiv(), this.svgAreas = [], this.svgAreasById = {}, this.drawChart()) : (this.chartDiv.style.textAlign = "", this.chartDiv.setAttribute("class", "ammapAlert"), this.chartDiv.innerHTML = this.svgNotSupported, this.fire("failed", {
                type: "failed",
                chart: this
            }))
        },
        storeTemp: function() {
            var a = this.zoomLongitude();
            isNaN(a) || (this.zLongTemp = a);
            a = this.zoomLatitude();
            isNaN(a) || (this.zLatTemp = a);
            a = this.zoomLevel();
            isNaN(a) || (this.zLevelTemp = a)
        },
        invalidateSize: function() {
            this.storeTemp();
            e.AmMap.base.invalidateSize.call(this)
        },
        validateSize: function() {
            e.hasSVG && 0 < this.realWidth && 0 < this.realHeight && this.storeTemp();
            e.AmMap.base.validateSize.call(this)
        },
        handleWheelReal: function(a) {
            if (!this.wheelBusy) {
                this.stopAnimation();
                var b = this.zoomLevel(),
                    c = this.zoomControl,
                    d = c.zoomFactor;
                this.wheelBusy = !0;
                a = e.fitToBounds(0 < a ? b * d : b / d, c.minZoomLevel, c.maxZoomLevel);
                d = this.mouseX / this.mapWidth;
                c = this.mouseY / this.mapHeight;
                d = (this.zoomX() - d) * (a / b) + d;
                b = (this.zoomY() - c) * (a / b) + c;
                this.zoomTo(a, d, b)
            }
        },
        addLegend: function(a, b) {
            a.position = "absolute";
            a.autoMargins = !1;
            a.valueWidth = 0;
            a.switchable = !1;
            e.AmMap.base.addLegend.call(this, a, b);
            void 0 === a.enabled && (a.enabled = !0);
            return a
        },
        handleLegendEvent: function() {},
        createDescriptionsDiv: function() {
            if (!this.descriptionsDiv) {
                var a = document.createElement("div"),
                    b = a.style;
                b.position = "absolute";
                b.left = "0px";
                b.top = "0px";
                this.descriptionsDiv = a
            }
            this.containerDiv.appendChild(this.descriptionsDiv)
        },
        drawChart: function() {
            e.AmMap.base.drawChart.call(this);
            var a = this.dataProvider;
            this.dataProvider = a = e.extend(a, new e.MapData, !0);
            this.areasSettings = e.processObject(this.areasSettings, e.AreasSettings, this.theme);
            this.imagesSettings = e.processObject(this.imagesSettings, e.ImagesSettings, this.theme);
            this.linesSettings = e.processObject(this.linesSettings, e.LinesSettings, this.theme);
            var b = this.container;
            this.mapContainer && this.mapContainer.remove();
            this.mapContainer = b.set();
            this.graphsSet.push(this.mapContainer);
            var c;
            a.map && (c = e.maps[a.map]);
            a.mapVar && (c = a.mapVar);
            c ? (this.svgData = c.svg, this.getBounds(), this.buildEverything()) : (a = a.mapURL) && this.loadXml(a);
            this.balloonsSet.toFront()
        },
        drawBg: function() {
            var a = this;
            a.background.click(function() {
                a.handleBackgroundClick()
            })
        },
        buildEverything: function() {
            if (0 < this.realWidth && 0 < this.realHeight) {
                var a = this.container;
                this.zoomControl = e.processObject(this.zoomControl, e.ZoomControl, this.theme);
                this.zoomControl.init(this, a);
                this.drawBg();
                this.buildSVGMap();
                var b = this.smallMap;
                b && (b = this.smallMap = e.processObject(this.smallMap, e.SmallMap, this.theme), b.init(this, a));
                b = this.dataProvider;
                isNaN(b.zoomX) && isNaN(b.zoomY) && isNaN(b.zoomLatitude) && isNaN(b.zoomLongitude) && (this.centerMap ? (b.zoomLatitude = this.coordinateToLatitude(this.mapHeight / 2), b.zoomLongitude = this.coordinateToLongitude(this.mapWidth / 2)) : (b.zoomX = 0, b.zoomY = 0), this.zoomInstantly = !0);
                this.selectObject(this.dataProvider);
                this.processAreas();
                if (b = this.valueLegend) this.valueLegend = b = e.processObject(b, e.ValueLegend, this.theme), b.init(this, a);
                this.objectList && (a = this.objectList = e.processObject(this.objectList, e.ObjectList)) && (this.clearObjectList(), a.init(this));
                this.dispDUpd();
                this.linkSet.toFront()
            } else this.cleanChart()
        },
        hideGroup: function(a) {
            this.showHideGroup(a, !1)
        },
        showGroup: function(a) {
            this.showHideGroup(a, !0)
        },
        showHideGroup: function(a, b) {
            this.showHideReal(this.imagesProcessor.allObjects, a, b);
            this.showHideReal(this.areasProcessor.allObjects, a, b);
            this.showHideReal(this.linesProcessor.allObjects, a, b)
        },
        showHideReal: function(a, b, c) {
            var d;
            for (d = 0; d < a.length; d++) {
                var f = a[d];
                if (f.groupId == b) {
                    var e = f.displayObject;
                    e && (c ? (f.hidden = !1, e.show()) : (f.hidden = !0, e.hide()))
                }
            }
        },
        update: function() {
            e.hasSVG && (e.AmMap.base.update.call(this), this.zoomControl && this.zoomControl.update && this.zoomControl.update())
        },
        animateMap: function() {
            var a = this;
            a.totalFrames = 1E3 * a.zoomDuration / e.updateRate;
            a.totalFrames += 1;
            a.frame = 0;
            a.tweenPercent = 0;
            a.balloon.hide(0);
            setTimeout(function() {
                a.updateSize.call(a)
            }, e.updateRate)
        },
        updateSize: function() {
            var a = this,
                b = a.totalFrames;
            a.preventHover = !0;
            a.frame <= b ? (a.frame++, b = e.easeOutSine(0, a.frame, 0, 1, b), 1 <= b ? (b = 1, a.preventHover = !1, a.wheelBusy = !1) : setTimeout(function() {
                a.updateSize.call(a)
            }, e.updateRate), .8 < b && (a.preventHover = !1)) : (b = 1, a.preventHover = !1, a.wheelBusy = !1);
            a.tweenPercent = b;
            a.rescaleMapAndObjects()
        },
        rescaleMapAndObjects: function() {
            var a = this.initialScale,
                b = this.initialX,
                c = this.initialY,
                d = this.tweenPercent,
                a = a + (this.finalScale - a) * d;
            this.mapContainer.translate(b + (this.finalX - b) * d, c + (this.finalY - c) * d, a, !0);
            if (this.areasSettings.adjustOutlineThickness) {
                for (var b = this.svgAreas, f = 0; f < b.length; f++)(c = b[f]) && c.setAttr("stroke-width", this.areasSettings.outlineThickness / a);
                for (f = 0; f < b.length; f++) {
                    var c = b[f],
                        e = c.displayObject;
                    e && e.setAttr("stroke-width", c.outlineThicknessReal / a)
                }
            }
            this.rescaleObjects();
            this.positionChanged();
            this.updateSmallMap();
            1 == d && (d = {
                type: "zoomCompleted",
                chart: this
            }, this.fire(d.type, d))
        },
        updateSmallMap: function() {
            this.smallMap && this.smallMap.update()
        },
        rescaleObjects: function() {
            var a = this.mapContainer.scale,
                b = this.imagesProcessor.objectsToResize,
                c;
            for (c = 0; c < b.length; c++) {
                var d = b[c].image;
                d.translate(d.x, d.y, b[c].scale / a, !0)
            }
            b = this.imagesProcessor.labelsToReposition;
            for (c = 0; c < b.length; c++) d = b[c], d.imageLabel && this.imagesProcessor.positionLabel(d.imageLabel, d, d.labelPositionReal);
            b = this.linesProcessor;
            if (d = b.linesToResize)
                for (c = 0; c < d.length; c++) {
                    var f = d[c];
                    f.line.setAttr("stroke-width", f.thickness / a)
                }
            b = b.objectsToResize;
            for (c = 0; c < b.length; c++) d = b[c], d.translate(d.x, d.y, 1 / a, !0)
        },
        handleTouchEnd: function(a) {
            this.initialDistance = NaN;
            this.mouseIsDown = this.isDragging = !1;
            e.AmMap.base.handleTouchEnd.call(this, a)
        },
        handleMouseDown: function(a) {
            e.resetMouseOver();
            this.mouseIsDown = this.mouseIsOver = !0;
            this.balloon.hide(0);
            a && this.mouseIsOver && a.preventDefault && this.panEventsEnabled && a.preventDefault();
            if (this.chartCreated && !this.preventHover && (this.initialTouchZoom = this.zoomLevel(), this.dragMap && (this.stopAnimation(), this.mapContainerClickX = this.mapContainer.x, this.mapContainerClickY = this.mapContainer.y), a || (a = window.event), a.shiftKey && !0 === this.developerMode && this.getDevInfo(), a && a.touches)) {
                var b = this.mouseX,
                    c = this.mouseY,
                    d = a.touches.item(1);
                d && this.panEventsEnabled && this.boundingRect && (a = d.clientX - this.boundingRect.left, d = d.clientY - this.boundingRect.top, this.middleXP = (b + (a - b) / 2) / this.realWidth, this.middleYP = (c + (d - c) / 2) / this.realHeight, this.initialDistance =
                    Math.sqrt(Math.pow(a - b, 2) + Math.pow(d - c, 2)))
            }
        },
        stopDrag: function() {
            this.isDragging = !1
        },
        handleReleaseOutside: function() {
            if (e.isModern) {
                var a = this;
                a.mouseIsDown = !1;
                setTimeout(function() {
                    a.resetPinch.call(a)
                }, 100);
                if (!a.preventHover) {
                    a.stopDrag();
                    var b = a.zoomControl;
                    b && b.draggerUp && b.draggerUp();
                    a.mapWasDragged = !1;
                    var b = a.mapContainer,
                        c = a.mapContainerClickX,
                        d = a.mapContainerClickY;
                    isNaN(c) || isNaN(d) || !(2 < Math.abs(b.x - c) || Math.abs(b.y - d)) || (a.mapWasDragged = !0, b = {
                        type: "dragCompleted",
                        zoomX: a.zoomX(),
                        zoomY: a.zoomY(),
                        zoomLevel: a.zoomLevel(),
                        chart: a
                    }, a.fire(b.type, b));
                    if (a.mouseIsOver && !a.mapWasDragged && !a.skipClick || a.wasTouched && 4 > Math.abs(a.mouseX - a.tmx) && 4 > Math.abs(a.mouseY - a.tmy)) b = {
                        type: "click",
                        x: a.mouseX,
                        y: a.mouseY,
                        chart: a
                    }, a.fire(b.type, b);
                    a.mapContainerClickX = NaN;
                    a.mapContainerClickY = NaN;
                    a.objectWasClicked = !1;
                    a.zoomOnDoubleClick && a.mouseIsOver && (b = (new Date).getTime(), 200 > b - a.previousClickTime && 40 < b - a.previousClickTime && a.doDoubleClickZoom(), a.previousClickTime = b)
                }
                a.wasTouched = !1
            }
        },
        resetPinch: function() {
            this.mapWasPinched = !1
        },
        handleMouseMove: function(a) {
            var b = this;
            e.AmMap.base.handleMouseMove.call(b, a);
            if (!a || !a.touches || !b.tapToActivate || b.tapped) {
                b.panEventsEnabled && b.mouseIsOver && a && a.preventDefault && a.preventDefault();
                var c = b.previuosMouseX,
                    d = b.previuosMouseY,
                    f = b.mouseX,
                    k = b.mouseY,
                    g = b.zoomControl;
                isNaN(c) && (c = f);
                isNaN(d) && (d = k);
                b.mouse2X = NaN;
                b.mouse2Y = NaN;
                a && a.touches && (a = a.touches.item(1)) && b.panEventsEnabled && b.boundingRect && (b.mouse2X = a.clientX - b.boundingRect.left, b.mouse2Y = a.clientY - b.boundingRect.top);
                if (a = b.mapContainer) {
                    var h = b.mouse2X,
                        l = b.mouse2Y;
                    b.pinchTO && clearTimeout(b.pinchTO);
                    b.pinchTO = setTimeout(function() {
                        b.resetPinch.call(b)
                    }, 1E3);
                    var n = b.realHeight,
                        m = b.realWidth,
                        r = b.mapWidth,
                        u = b.mapHeight;
                    b.mouseIsDown && b.dragMap && (5 < Math.abs(b.previuosMouseX - b.mouseX) || 5 < Math.abs(b.previuosMouseY - b.mouseY)) && (b.isDragging = !0);
                    if (!isNaN(h)) {
                        b.stopDrag();
                        var q = Math.sqrt(Math.pow(h - f, 2) + Math.pow(l - k, 2)),
                            p = b.initialDistance;
                        isNaN(p) && (p = Math.sqrt(Math.pow(h - f, 2) + Math.pow(l - k, 2)));
                        if (!isNaN(p)) {
                            var h = b.initialTouchZoom * q / p,
                                h = e.fitToBounds(h, g.minZoomLevel, g.maxZoomLevel),
                                g = b.zoomLevel(),
                                p = b.middleXP,
                                l = b.middleYP,
                                q = n / u,
                                w = m / r,
                                p = (b.zoomX() - p * w) * (h / g) + p * w,
                                l = (b.zoomY() - l * q) * (h / g) + l * q;.1 < Math.abs(h - g) && (b.zoomTo(h, p, l, !0), b.mapWasPinched = !0, clearTimeout(b.pinchTO))
                        }
                    }
                    h = a.scale;
                    b.isDragging && (b.balloon.hide(0), b.positionChanged(), c = a.x + (f - c), d = a.y + (k - d), b.preventDragOut && (u = -u * h + n / 2, n /= 2, c = e.fitToBounds(c, -r * h + m / 2, m / 2), d = e.fitToBounds(d, u, n)), a.translate(c, d, h, !0), b.updateSmallMap());
                    b.previuosMouseX = f;
                    b.previuosMouseY = k
                }
            }
        },
        selectObject: function(a, b) {
            var c = this;
            (new Date).getTime();
            a || (a = c.dataProvider);
            a.isOver = !1;
            var d = a.linkToObject;
            "string" == typeof d && (d = c.getObjectById(d));
            a.useTargetsZoomValues && d && (a.zoomX = d.zoomX, a.zoomY = d.zoomY, a.zoomLatitude = d.zoomLatitude, a.zoomLongitude = d.zoomLongitude, a.zoomLevel = d.zoomLevel);
            var f = c.selectedObject;
            f && c.returnInitialColor(f);
            c.selectedObject = a;
            var k = !1,
                g, h;
            "MapArea" == a.objectType && (a.autoZoomReal && (k = !0), g = c.areasSettings.selectedOutlineColor, h = c.areasSettings.selectedOutlineThickness);
            if (d && !k && ("string" == typeof d && (d = c.getObjectById(d)), isNaN(a.zoomLevel) && isNaN(a.zoomX) && isNaN(a.zoomY))) {
                if (c.extendMapData(d)) return;
                c.selectObject(d);
                return
            }
            c.allowMultipleDescriptionWindows || c.closeAllDescriptions();
            clearTimeout(c.selectedObjectTimeOut);
            clearTimeout(c.processObjectsTimeOut);
            d = c.zoomDuration;
            !k && isNaN(a.zoomLevel) && isNaN(a.zoomX) && isNaN(a.zoomY) ? (c.showDescriptionAndGetUrl(), b || c.processObjects()) : (c.selectedObjectTimeOut = setTimeout(function() {
                c.showDescriptionAndGetUrl.call(c)
            }, 1E3 * d + 200), c.showObjectsAfterZoom) ? b || (c.processObjectsTimeOut = setTimeout(function() {
                c.processObjects.call(c)
            }, 1E3 * d + 200)) : b || c.processObjects();
            d = a.displayObject;
            k = a.selectedColorReal;
            "MapImage" == a.objectType && (g = c.imagesSettings.selectedOutlineColor, h = c.imagesSettings.selectedOutlineThickness, d = a.image);
            if (d) {
                if (e.setCN(c, d, "selected-object"), a.bringForwardOnHover && d.toFront(), !a.preserveOriginalAttributes) {
                    d.setAttr("stroke", a.outlineColorReal);
                    void 0 !== k && d.setAttr("fill", k);
                    void 0 !== g && d.setAttr("stroke", g);
                    void 0 !== h && d.setAttr("stroke-width", h);
                    if ("MapLine" == a.objectType) {
                        var l = a.lineSvg;
                        l && l.setAttr("stroke", k);
                        if (l = a.arrowSvg) l.setAttr("fill", k), l.setAttr("stroke", k)
                    }
                    if (l = a.imageLabel) {
                        var n = a.selectedLabelColorReal;
                        void 0 !== n && l.setAttr("fill", n)
                    }
                    a.selectable || (d.setAttr("cursor", "default"), l && l.setAttr("cursor", "default"))
                }
            } else c.returnInitialColorReal(a);
            if (d = a.groupId)
                for (l = a.groupArray, l || (l = c.getGroupById(d), a.groupArray = l), n = 0; n < l.length; n++) {
                    var m = l[n];
                    m.isOver = !1;
                    d = m.displayObject;
                    "MapImage" == m.objectType && (d = m.image);
                    if (d) {
                        var r = m.selectedColorReal;
                        void 0 !== r && d.setAttr("fill", r);
                        void 0 !== g && d.setAttr("stroke", g);
                        void 0 !== h && d.setAttr("stroke-width", h);
                        "MapLine" == m.objectType && ((d = m.lineSvg) && d.setAttr("stroke", k), d = m.arrowSvg) && (d.setAttr("fill", k), d.setAttr("stroke", k))
                    }
                }
            c.zoomToSelectedObject();
            f != a && (f = {
                type: "selectedObjectChanged",
                chart: c
            }, c.fire(f.type, f))
        },
        returnInitialColor: function(a, b) {
            this.returnInitialColorReal(a);
            b && (a.isFirst = !1);
            if (this.selectedObject.bringForwardOnHover) {
                var c = this.selectedObject.displayObject;
                c && c.toFront()
            }
            if (c = a.groupId) {
                var c = this.getGroupById(c),
                    d;
                for (d = 0; d < c.length; d++) this.returnInitialColorReal(c[d]), b && (c[d].isFirst = !1)
            }
        },
        closeAllDescriptions: function() {
            this.descriptionsDiv.innerHTML = ""
        },
        returnInitialColorReal: function(a) {
            a.isOver = !1;
            var b = a.displayObject;
            if (b) {
                b.toPrevious();
                if ("MapImage" == a.objectType) {
                    var c = a.tempScale;
                    isNaN(c) || b.translate(b.x, b.y, c, !0);
                    a.tempScale = NaN;
                    b = a.image
                }
                c = a.colorReal;
                if ("MapLine" == a.objectType) {
                    var d = a.lineSvg;
                    d && d.setAttr("stroke", c);
                    if (d = a.arrowSvg) d.setAttr("fill", c), d.setAttr("stroke", c)
                }
                var d = a.alphaReal,
                    f = a.outlineAlphaReal,
                    e = a.outlineThicknessReal,
                    g = a.outlineColorReal;
                if (a.showAsSelected) {
                    var c = a.selectedColorReal,
                        h, l;
                    "MapImage" == a.objectType && (h = this.imagesSettings.selectedOutlineColor, l = this.imagesSettings.selectedOutlineThickness);
                    "MapArea" == a.objectType && (h = this.areasSettings.selectedOutlineColor, l = this.areasSettings.selectedOutlineThickness);
                    void 0 !== h && (g = h);
                    void 0 !== l && (e = l)
                }
                "bubble" == a.type && (c = void 0);
                void 0 !== c && b.setAttr("fill", c);
                if (h = a.image) h.setAttr("fill", c), h.setAttr("stroke", g), h.setAttr("stroke-width", e), h.setAttr("fill-opacity", d), h.setAttr("stroke-opacity", f);
                "MapArea" == a.objectType && (c = 1, this.areasSettings.adjustOutlineThickness && (c = this.zoomLevel()), b.setAttr("stroke", g), b.setAttr("stroke-width", e / c), b.setAttr("fill-opacity", d), b.setAttr("stroke-opacity", f));
                (c = a.pattern) && b.pattern(c, this.mapScale, this.path);
                (b = a.imageLabel) && !a.labelInactive && (a.showAsSelected && void 0 !== a.selectedLabelColor ? b.setAttr("fill", a.selectedLabelColor) : b.setAttr("fill", a.labelColorReal))
            }
        },
        zoomToRectangle: function(a, b, c, d) {
            var f = this.realWidth,
                k = this.realHeight,
                g = this.mapSet.scale,
                h = this.zoomControl,
                f = e.fitToBounds(c / f > d / k ? .8 * f / (c * g) : .8 * k / (d * g), h.minZoomLevel, h.maxZoomLevel);
            this.zoomToMapXY(f, (a + c / 2) * g, (b + d / 2) * g)
        },
        zoomToLatLongRectangle: function(a, b, c, d) {
            var f = this.dataProvider,
                k = this.zoomControl,
                g = Math.abs(c - a),
                h = Math.abs(b - d),
                l = Math.abs(f.rightLongitude - f.leftLongitude),
                f = Math.abs(f.topLatitude -
                    f.bottomLatitude),
                k = e.fitToBounds(g / l > h / f ? .8 * l / g : .8 * f / h, k.minZoomLevel, k.maxZoomLevel);
            this.zoomToLongLat(k, a + (c - a) / 2, d + (b - d) / 2)
        },
        getGroupById: function(a) {
            var b = [];
            this.getGroup(this.imagesProcessor.allObjects, a, b);
            this.getGroup(this.linesProcessor.allObjects, a, b);
            this.getGroup(this.areasProcessor.allObjects, a, b);
            return b
        },
        zoomToGroup: function(a) {
            a = "object" == typeof a ? a : this.getGroupById(a);
            var b, c, d, f, e;
            for (e = 0; e < a.length; e++) {
                var g = a[e].displayObject;
                if (g) {
                    var h = g.getBBox(),
                        g = h.y,
                        l = h.y + h.height,
                        n = h.x,
                        h = h.x + h.width;
                    if (g < b || isNaN(b)) b = g;
                    if (l > f || isNaN(f)) f = l;
                    if (n < c || isNaN(c)) c = n;
                    if (h > d || isNaN(d)) d = h
                }
            }
            a = this.mapSet.getBBox();
            c -= a.x;
            d -= a.x;
            f -= a.y;
            b -= a.y;
            this.zoomToRectangle(c, b, d - c, f - b)
        },
        getGroup: function(a, b, c) {
            if (a) {
                var d;
                for (d = 0; d < a.length; d++) {
                    var f = a[d];
                    f.groupId == b && c.push(f)
                }
            }
        },
        zoomToStageXY: function(a, b, c, d) {
            if (!this.objectWasClicked) {
                var f = this.zoomControl;
                a = e.fitToBounds(a, f.minZoomLevel, f.maxZoomLevel);
                f = this.zoomLevel();
                c = this.coordinateToLatitude((c - this.mapContainer.y) / f);
                b = this.coordinateToLongitude((b -
                    this.mapContainer.x) / f);
                this.zoomToLongLat(a, b, c, d)
            }
        },
        zoomToLongLat: function(a, b, c, d) {
            b = this.longitudeToCoordinate(b);
            c = this.latitudeToCoordinate(c);
            this.zoomToMapXY(a, b, c, d)
        },
        zoomToMapXY: function(a, b, c, d) {
            var f = this.mapWidth,
                e = this.mapHeight;
            this.zoomTo(a, -(b / f) * a + this.realWidth / f / 2, -(c / e) * a + this.realHeight / e / 2, d)
        },
        zoomToObject: function(a) {
            if (a) {
                var b = a.zoomLatitude,
                    c = a.zoomLongitude,
                    d = a.zoomLevel,
                    f = this.zoomInstantly,
                    k = a.zoomX,
                    g = a.zoomY,
                    h = this.realWidth,
                    l = this.realHeight;
                isNaN(d) || (isNaN(b) || isNaN(c) ?
                    this.zoomTo(d, k, g, f) : this.zoomToLongLat(d, c, b, f));
                this.zoomInstantly = !1;
                "MapImage" == a.objectType && isNaN(a.zoomX) && isNaN(a.zoomY) && isNaN(a.zoomLatitude) && isNaN(a.zoomLongitude) && !isNaN(a.latitude) && !isNaN(a.longitude) && this.zoomToLongLat(a.zoomLevel, a.longitude, a.latitude);
                "MapArea" == a.objectType && (k = a.displayObject.getBBox(), b = this.mapScale, c = k.x * b, d = k.y * b, f = k.width * b, k = k.height * b, h = a.autoZoomReal && isNaN(a.zoomLevel) ? f / h > k / l ? .8 * h / f : .8 * l / k : a.zoomLevel, l = this.zoomControl, h = e.fitToBounds(h, l.minZoomLevel, l.maxZoomLevel), isNaN(a.zoomX) && isNaN(a.zoomY) && isNaN(a.zoomLatitude) && isNaN(a.zoomLongitude) && (a = this.mapSet.getBBox(), this.zoomToMapXY(h, -a.x * b + c + f / 2, -a.y * b + d + k / 2)));
                this.zoomControl.update()
            }
        },
        zoomToSelectedObject: function() {
            this.zoomToObject(this.selectedObject)
        },
        zoomTo: function(a, b, c, d) {
            var f = this.zoomControl;
            a = e.fitToBounds(a, f.minZoomLevel, f.maxZoomLevel);
            f = this.zoomLevel();
            isNaN(b) && (b = this.realWidth / this.mapWidth, b = (this.zoomX() - .5 * b) * (a / f) + .5 * b);
            isNaN(c) && (c = this.realHeight / this.mapHeight, c = (this.zoomY() - .5 * c) * (a / f) + .5 * c);
            this.stopAnimation();
            isNaN(a) || (f = this.mapContainer, this.initialX = f.x, this.initialY = f.y, this.initialScale = f.scale, this.finalX = this.mapWidth * b, this.finalY = this.mapHeight * c, this.finalScale = a, this.finalX != this.initialX || this.finalY != this.initialY || this.finalScale != this.initialScale ? d ? (this.tweenPercent = 1, this.rescaleMapAndObjects(), this.wheelBusy = !1) : this.animateMap() : this.wheelBusy = !1)
        },
        loadXml: function(a) {
            var b;
            window.XMLHttpRequest && (b = new XMLHttpRequest);
            b.overrideMimeType && b.overrideMimeType("text/xml");
            b.open("GET", a, !1);
            b.send();
            this.parseXMLObject(b.responseXML);
            this.svgData && this.buildEverything()
        },
        stopAnimation: function() {
            this.frame = this.totalFrames
        },
        processObjects: function() {
            var a = this.selectedObject;
            if (0 < a.images.length || 0 < a.areas.length || 0 < a.lines.length || a == this.dataProvider) {
                var b = this.container,
                    c = this.stageImagesContainer;
                c && c.remove();
                this.stageImagesContainer = c = b.set();
                this.trendLinesSet.push(c);
                var d = this.stageLinesContainer;
                d && d.remove();
                this.stageLinesContainer = d = b.set();
                this.trendLinesSet.push(d);
                var f = this.mapImagesContainer;
                f && f.remove();
                this.mapImagesContainer = f = b.set();
                this.mapContainer.push(f);
                var e = this.mapLinesContainer;
                e && e.remove();
                this.mapLinesContainer = e = b.set();
                this.mapContainer.push(e);
                this.linesAboveImages ? (f.toFront(), c.toFront(), e.toFront(), d.toFront()) : (e.toFront(), d.toFront(), f.toFront(), c.toFront());
                a && (this.imagesProcessor.reset(), this.linesProcessor.reset(), this.linesAboveImages ? (this.imagesProcessor.process(a), this.linesProcessor.process(a)) : (this.linesProcessor.process(a), this.imagesProcessor.process(a)));
                this.rescaleObjects()
            }
        },
        processAreas: function() {
            this.areasProcessor.process(this.dataProvider)
        },
        buildSVGMap: function() {
            var a = this.svgData.g.path,
                b = this.container,
                c = b.set();
            void 0 === a.length && (a = [a]);
            var d;
            for (d = 0; d < a.length; d++) {
                var f = a[d],
                    e = f.d,
                    g = f.title;
                f.titleTr && (g = f.titleTr);
                e = b.path(e);
                e.id = f.id;
                if (this.areasSettings.preserveOriginalAttributes) {
                    e.customAttr = {};
                    for (var h in f) "d" != h && "id" != h && "title" != h && (e.customAttr[h] = f[h])
                }
                this.svgAreasById[f.id] = {
                    area: e,
                    title: g,
                    className: f["class"]
                };
                this.svgAreas.push(e);
                c.push(e)
            }
            this.mapSet = c;
            this.mapContainer.push(c);
            this.resizeMap()
        },
        addObjectEventListeners: function(a, b) {
            var c = this;
            a.mousedown(function(a) {
                c.mouseDownMapObject(b, a)
            }).mouseup(function(a) {
                c.clickMapObject(b, a)
            }).mouseover(function(a) {
                c.balloonX = NaN;
                c.rollOverMapObject(b, !0, a)
            }).mouseout(function(a) {
                c.balloonX = NaN;
                c.rollOutMapObject(b, a)
            }).touchend(function(a) {
                c.tapToActivate && !c.tapped || c.mapWasDragged || c.mapWasPinched || (c.balloonX = NaN, c.rollOverMapObject(b, !0, a), c.clickMapObject(b, a))
            }).touchstart(function(a) {
                c.mouseDownMapObject(b, a)
            })
        },
        checkIfSelected: function(a) {
            var b = this.selectedObject;
            if (b == a) return !0;
            if (b = b.groupId) {
                var b = this.getGroupById(b),
                    c;
                for (c = 0; c < b.length; c++)
                    if (b[c] == a) return !0
            }
            return !1
        },
        clearMap: function() {
            this.chartDiv.innerHTML = "";
            this.clearObjectList()
        },
        clearObjectList: function() {
            var a = this.objectList;
            a && a.div && (a.div.innerHTML = "")
        },
        checkIfLast: function(a) {
            if (a) {
                var b = a.parentNode;
                if (b && b.lastChild == a) return !0
            }
            return !1
        },
        showAsRolledOver: function(a) {
            var b = a.displayObject;
            if (!a.showAsSelected && b && !a.isOver) {
                b.node.onmouseout = function() {};
                b.node.onmouseover = function() {};
                b.node.onclick = function() {};
                !a.isFirst && a.bringForwardOnHover && (b.toFront(), a.isFirst = !0);
                var c = a.rollOverColorReal,
                    d;
                a.preserveOriginalAttributes && (c = void 0);
                void 0 == c && (isNaN(a.rollOverBrightnessReal) || (c = e.adjustLuminosity(a.colorReal, a.rollOverBrightnessReal / 100)));
                if (void 0 != c)
                    if ("MapImage" == a.objectType)(d = a.image) && d.setAttr("fill", c);
                    else if ("MapLine" == a.objectType) {
                    if ((d = a.lineSvg) && d.setAttr("stroke", c), d = a.arrowSvg) d.setAttr("fill", c), d.setAttr("stroke", c)
                } else b.setAttr("fill", c);
                (c = a.imageLabel) && !a.labelInactive && (d = a.labelRollOverColorReal, void 0 != d && c.setAttr("fill", d));
                c = a.rollOverOutlineColorReal;
                void 0 != c && ("MapImage" == a.objectType ? (d = a.image) && d.setAttr("stroke", c) : b.setAttr("stroke", c));
                "MapImage" == a.objectType ? (c = this.imagesSettings.rollOverOutlineThickness, (d = a.image) && (isNaN(c) || d.setAttr("stroke-width", c))) : (c = this.areasSettings.rollOverOutlineThickness, isNaN(c) || b.setAttr("stroke-width", c));
                if ("MapArea" == a.objectType) {
                    c = this.areasSettings;
                    d = a.rollOverAlphaReal;
                    isNaN(d) || b.setAttr("fill-opacity", d);
                    d = c.rollOverOutlineAlpha;
                    isNaN(d) || b.setAttr("stroke-opacity", d);
                    d = 1;
                    this.areasSettings.adjustOutlineThickness && (d = this.zoomLevel());
                    var f = c.rollOverOutlineThickness;
                    isNaN(f) || b.setAttr("stroke-width", f / d);
                    (c = c.rollOverPattern) && b.pattern(c, this.mapScale, this.path)
                }
                "MapImage" == a.objectType && (c = a.rollOverScaleReal, isNaN(c) || 1 == c || (d = b.scale, isNaN(d) && (d = 1), a.tempScale = d, b.translate(b.x, b.y, d * c, !0)));
                this.useHandCursorOnClickableOjects && this.checkIfClickable(a) && b.setAttr("cursor", "pointer");
                a.mouseEnabled && this.addObjectEventListeners(b, a);
                a.isOver = !0
            }
        },
        rollOverMapObject: function(a, b, c) {
            if (this.chartCreated) {
                this.handleMouseMove();
                var d = this.previouslyHovered;
                d && d != a ? (!1 === this.checkIfSelected(d) && (this.returnInitialColor(d, !0), this.previouslyHovered = null), this.balloon.hide(0)) : clearTimeout(this.hoverInt);
                if (!this.preventHover) {
                    if (!1 === this.checkIfSelected(a)) {
                        if (d = a.groupId) {
                            var d = this.getGroupById(d),
                                f;
                            for (f = 0; f < d.length; f++) d[f] != a && this.showAsRolledOver(d[f])
                        }
                        this.showAsRolledOver(a)
                    } else(d = a.displayObject) && (this.allowClickOnSelectedObject ? d.setAttr("cursor", "pointer") : d.setAttr("cursor", "default"));
                    this.showDescriptionOnHover ? this.showDescription(a) : !this.showBalloonOnSelectedObject && this.checkIfSelected(a) || !1 === b || (f = this.balloon, this.balloon.fixedPosition = !1, b = a.colorReal, d = "", void 0 !== b && this.useObjectColorForBalloon || (b = f.fillColor), (f = a.balloonTextReal) && (d = this.formatString(f, a)), this.balloonLabelFunction && (d = this.balloonLabelFunction(a, this)), "MapArea" != a.objectType && (this.balloonX = NaN), d && "" !== d && this.showBalloon(d, b, !1, this.balloonX, this.balloonY));
                    c = {
                        type: "rollOverMapObject",
                        mapObject: a,
                        chart: this,
                        event: c
                    };
                    this.fire(c.type, c);
                    this.previouslyHovered = a
                }
            }
        },
        longitudeToX: function(a) {
            return this.longitudeToCoordinate(a) * this.zoomLevel() + this.mapContainer.x
        },
        latitudeToY: function(a) {
            return this.latitudeToCoordinate(a) * this.zoomLevel() + this.mapContainer.y
        },
        latitudeToStageY: function(a) {
            return this.latitudeToCoordinate(a) * this.zoomLevel() + this.mapContainer.y
        },
        longitudeToStageX: function(a) {
            return this.longitudeToCoordinate(a) * this.zoomLevel() + this.mapContainer.x
        },
        stageXToLongitude: function(a) {
            a = (a - this.mapContainer.x) / this.zoomLevel();
            return this.coordinateToLongitude(a)
        },
        stageYToLatitude: function(a) {
            a = (a - this.mapContainer.y) / this.zoomLevel();
            return this.coordinateToLatitude(a)
        },
        rollOutMapObject: function(a, b) {
            this.hideBalloon();
            if (this.chartCreated && a.isOver) {
                this.checkIfSelected(a) ||
                    this.returnInitialColor(a);
                var c = {
                    type: "rollOutMapObject",
                    mapObject: a,
                    chart: this,
                    event: b
                };
                this.fire(c.type, c)
            }
        },
        formatString: function(a, b) {
            var c = this.nf,
                d = this.pf,
                f = b.title;
            b.titleTr && (f = b.titleTr);
            void 0 == f && (f = "");
            var k = b.value,
                k = isNaN(k) ? "" : e.formatNumber(k, c),
                c = b.percents,
                c = isNaN(c) ? "" : e.formatNumber(c, d),
                d = b.description;
            void 0 == d && (d = "");
            var g = b.customData;
            void 0 == g && (g = "");
            return a = e.massReplace(a, {
                "[[title]]": f,
                "[[value]]": k,
                "[[percent]]": c,
                "[[description]]": d,
                "[[customData]]": g
            })
        },
        mouseDownMapObject: function(a, b) {
            var c = {
                type: "mouseDownMapObject",
                mapObject: a,
                chart: this,
                event: b
            };
            this.fire(c.type, c)
        },
        clickMapObject: function(a, b) {
            var c = this;
            b && (b.touches || c.hideBalloon());
            if (c.chartCreated && !c.preventHover && !c.mapWasDragged && c.checkIfClickable(a) && !c.mapWasPinched) {
                c.selectObject(a);
                var d = c.zoomLevel();
                c.clickLatitude = c.coordinateToLatitude((c.mouseY - c.mapContainer.y) / d);
                c.clickLongitude = c.coordinateToLongitude((c.mouseX - c.mapContainer.x) / d);
                b && b.touches && setTimeout(function() {
                        c.showBalloonAfterZoom.call(c)
                    },
                    1E3 * c.zoomDuration);
                d = {
                    type: "clickMapObject",
                    mapObject: a,
                    chart: c,
                    event: b
                };
                c.fire(d.type, d);
                c.objectWasClicked = !0
            }
        },
        showBalloonAfterZoom: function() {
            this.balloonX = this.longitudeToX(this.clickLongitude);
            this.balloonY = this.latitudeToY(this.clickLatitude);
            this.rollOverMapObject(this.selectedObject, !0)
        },
        checkIfClickable: function(a) {
            var b = this.allowClickOnSelectedObject;
            return this.selectedObject == a && b ? !0 : this.selectedObject != a || b ? !0 === a.selectable || "MapArea" == a.objectType && a.autoZoomReal || a.url || a.linkToObject || 0 < a.images.length || 0 < a.lines.length || !isNaN(a.zoomLevel) || !isNaN(a.zoomX) || !isNaN(a.zoomY) || a.description ? !0 : !1 : !1
        },
        resizeMap: function() {
            var a = this.mapSet;
            if (a) {
                var b = 1,
                    c = a.getBBox(),
                    d = this.realWidth,
                    f = this.realHeight,
                    e = c.width,
                    g = c.height;
                this.fitMapToContainer && (b = e / d > g / f ? d / e : f / g);
                a.translate(-c.x * b, -c.y * b, b, !0);
                this.mapScale = b;
                this.mapHeight = g * b;
                this.mapWidth = e * b
            }
        },
        zoomIn: function() {
            var a = this.zoomLevel() * this.zoomControl.zoomFactor;
            this.zoomTo(a)
        },
        zoomOut: function() {
            var a = this.zoomLevel() / this.zoomControl.zoomFactor;
            this.zoomTo(a)
        },
        moveLeft: function() {
            var a = this.zoomX() + this.zoomControl.panStepSize;
            this.zoomTo(this.zoomLevel(), a, this.zoomY())
        },
        moveRight: function() {
            var a = this.zoomX() - this.zoomControl.panStepSize;
            this.zoomTo(this.zoomLevel(), a, this.zoomY())
        },
        moveUp: function() {
            var a = this.zoomY() + this.zoomControl.panStepSize;
            this.zoomTo(this.zoomLevel(), this.zoomX(), a)
        },
        moveDown: function() {
            var a = this.zoomY() - this.zoomControl.panStepSize;
            this.zoomTo(this.zoomLevel(), this.zoomX(), a)
        },
        zoomX: function() {
            return this.mapSet ? Math.round(1E4 * this.mapContainer.x / this.mapWidth) / 1E4 : NaN
        },
        zoomY: function() {
            return this.mapSet ? Math.round(1E4 * this.mapContainer.y / this.mapHeight) / 1E4 : NaN
        },
        goHome: function() {
            this.selectObject(this.dataProvider);
            var a = {
                type: "homeButtonClicked",
                chart: this
            };
            this.fire(a.type, a)
        },
        zoomLevel: function() {
            return Math.round(1E5 * this.mapContainer.scale) / 1E5
        },
        showDescriptionAndGetUrl: function() {
            var a = this.selectedObject;
            if (a) {
                this.showDescription();
                var b = a.url;
                if (b) e.getURL(b, a.urlTarget);
                else if (b = a.linkToObject) {
                    if ("string" ==
                        typeof b) {
                        var c = this.getObjectById(b);
                        if (c) {
                            this.selectObject(c);
                            return
                        }
                    }
                    b && a.passZoomValuesToTarget && (b.zoomLatitude = this.zoomLatitude(), b.zoomLongitude = this.zoomLongitude(), b.zoomLevel = this.zoomLevel());
                    this.extendMapData(b) || this.selectObject(b)
                }
            }
        },
        extendMapData: function(a) {
            var b = a.objectType;
            if ("MapImage" != b && "MapArea" != b && "MapLine" != b) return e.extend(a, new e.MapData, !0), this.dataProvider = a, this.zoomInstantly = !0, this.validateData(), !0
        },
        showDescription: function(a) {
            a || (a = this.selectedObject);
            this.allowMultipleDescriptionWindows || this.closeAllDescriptions();
            if (a.description) {
                var b = a.descriptionWindow;
                b && b.close();
                b = new e.DescriptionWindow;
                a.descriptionWindow = b;
                var c = a.descriptionWindowWidth,
                    d = a.descriptionWindowHeight,
                    f = a.descriptionWindowLeft,
                    k = a.descriptionWindowTop,
                    g = a.descriptionWindowRight,
                    h = a.descriptionWindowBottom;
                isNaN(g) || (f = this.realWidth - g);
                isNaN(h) || (k = this.realHeight - h);
                var l = a.descriptionWindowX;
                isNaN(l) || (f = l);
                l = a.descriptionWindowY;
                isNaN(l) || (k = l);
                isNaN(f) && (f = this.mouseX, f = f > this.realWidth / 2 ? f - c - 20 : f + 20);
                isNaN(k) && (k = this.mouseY);
                b.maxHeight = d;
                l = a.title;
                a.titleTr && (l = a.titleTr);
                b.show(this, this.descriptionsDiv, a.description, l);
                a = b.div.style;
                a.position = "absolute";
                a.width = c + "px";
                a.maxHeight = d + "px";
                isNaN(h) || (k -= b.div.offsetHeight);
                isNaN(g) || (f -= b.div.offsetWidth);
                a.left = f + "px";
                a.top = k + "px"
            }
        },
        parseXMLObject: function(a) {
            var b = {
                root: {}
            };
            this.parseXMLNode(b, "root", a);
            this.svgData = b.root.svg;
            this.getBounds()
        },
        getBounds: function() {
            var a = this.dataProvider;
            try {
                var b = this.svgData.defs["amcharts:ammap"];
                a.leftLongitude = Number(b.leftLongitude);
                a.rightLongitude = Number(b.rightLongitude);
                a.topLatitude = Number(b.topLatitude);
                a.bottomLatitude = Number(b.bottomLatitude);
                a.projection = b.projection;
                var c = b.wrappedLongitudes;
                c && (a.rightLongitude += 360);
                a.wrappedLongitudes = c
            } catch (d) {}
        },
        recalcLongitude: function(a) {
            var b = this.dataProvider.leftLongitude,
                c = this.dataProvider.wrappedLongitudes;
            return isNaN(a) && c ? a < b ? Number(a) + 360 : a : a
        },
        latitudeToCoordinate: function(a) {
            var b, c = this.dataProvider;
            if (this.mapSet) {
                b = c.topLatitude;
                var d = c.bottomLatitude;
                "mercator" == c.projection && (a = this.mercatorLatitudeToCoordinate(a), b = this.mercatorLatitudeToCoordinate(b), d = this.mercatorLatitudeToCoordinate(d));
                b = (a - b) / (d - b) * this.mapHeight
            }
            return b
        },
        longitudeToCoordinate: function(a) {
            a = this.recalcLongitude(a);
            var b, c = this.dataProvider;
            this.mapSet && (b = c.leftLongitude, b = (a - b) / (c.rightLongitude - b) * this.mapWidth);
            return b
        },
        mercatorLatitudeToCoordinate: function(a) {
            89.5 < a && (a = 89.5); - 89.5 > a && (a = -89.5);
            a = e.degreesToRadians(a);
            return e.radiansToDegrees(.5 * Math.log((1 + Math.sin(a)) /
                (1 - Math.sin(a))) / 2)
        },
        zoomLatitude: function() {
            if (this.mapContainer) return this.coordinateToLatitude((-this.mapContainer.y + this.previousHeight / 2) / this.zoomLevel())
        },
        zoomLongitude: function() {
            if (this.mapContainer) return this.coordinateToLongitude((-this.mapContainer.x + this.previousWidth / 2) / this.zoomLevel())
        },
        getAreaCenterLatitude: function(a) {
            a = a.displayObject.getBBox();
            var b = this.mapScale;
            a = -this.mapSet.getBBox().y * b + (a.y + a.height / 2) * b;
            return this.coordinateToLatitude(a)
        },
        getAreaCenterLongitude: function(a) {
            a =
                a.displayObject.getBBox();
            var b = this.mapScale;
            a = -this.mapSet.getBBox().x * b + (a.x + a.width / 2) * b;
            return this.coordinateToLongitude(a)
        },
        coordinateToLatitude: function(a) {
            var b;
            if (this.mapSet) {
                var c = this.dataProvider,
                    d = c.bottomLatitude,
                    f = c.topLatitude;
                b = this.mapHeight;
                "mercator" == c.projection ? (c = this.mercatorLatitudeToCoordinate(d), f = this.mercatorLatitudeToCoordinate(f), b = e.radiansToDegrees(2 * Math.atan(Math.exp(2 * (a * (c - f) / b + f) * Math.PI / 180)) - .5 * Math.PI)) : b = a / b * (d - f) + f
            }
            return Math.round(1E6 * b) / 1E6
        },
        coordinateToLongitude: function(a) {
            var b,
                c = this.dataProvider;
            this.mapSet && (b = a / this.mapWidth * (c.rightLongitude - c.leftLongitude) + c.leftLongitude);
            return Math.round(1E6 * b) / 1E6
        },
        milesToPixels: function(a) {
            var b = this.dataProvider;
            return this.mapWidth / (b.rightLongitude - b.leftLongitude) * a / 69.172
        },
        kilometersToPixels: function(a) {
            var b = this.dataProvider;
            return this.mapWidth / (b.rightLongitude - b.leftLongitude) * a / 111.325
        },
        handleBackgroundClick: function() {
            if (this.backgroundZoomsToTop && !this.mapWasDragged) {
                var a = this.dataProvider;
                if (this.checkIfClickable(a)) this.clickMapObject(a);
                else {
                    var b = a.zoomX,
                        c = a.zoomY,
                        d = a.zoomLongitude,
                        f = a.zoomLatitude,
                        a = a.zoomLevel;
                    isNaN(b) || isNaN(c) || this.zoomTo(a, b, c);
                    isNaN(d) || isNaN(f) || this.zoomToLongLat(a, d, f, !0)
                }
            }
        },
        parseXMLNode: function(a, b, c, d) {
            void 0 === d && (d = "");
            var f, e, g;
            if (c) {
                var h = c.childNodes.length;
                for (f = 0; f < h; f++) {
                    e = c.childNodes[f];
                    var l = e.nodeName,
                        n = e.nodeValue ? this.trim(e.nodeValue) : "",
                        m = !1;
                    e.attributes && 0 < e.attributes.length && (m = !0);
                    if (0 !== e.childNodes.length || "" !== n || !1 !== m)
                        if (3 == e.nodeType || 4 == e.nodeType) {
                            if ("" !== n) {
                                e = 0;
                                for (g in a[b]) a[b].hasOwnProperty(g) && e++;
                                e ? a[b]["#text"] = n : a[b] = n
                            }
                        } else if (1 == e.nodeType) {
                        var r;
                        void 0 !== a[b][l] ? void 0 === a[b][l].length ? (r = a[b][l], a[b][l] = [], a[b][l].push(r), a[b][l].push({}), r = a[b][l][1]) : "object" == typeof a[b][l] && (a[b][l].push({}), r = a[b][l][a[b][l].length - 1]) : (a[b][l] = {}, r = a[b][l]);
                        if (e.attributes && e.attributes.length)
                            for (n = 0; n < e.attributes.length; n++) r[e.attributes[n].name] = e.attributes[n].value;
                        void 0 !== a[b][l].length ? this.parseXMLNode(a[b][l], a[b][l].length - 1, e, d + "  ") : this.parseXMLNode(a[b], l, e, d + "  ")
                    }
                }
                e = 0;
                c = "";
                for (g in a[b]) "#text" == g ? c = a[b][g] : e++;
                0 === e && void 0 === a[b].length && (a[b] = c)
            }
        },
        doDoubleClickZoom: function() {
            if (!this.mapWasDragged) {
                var a = this.zoomLevel() * this.zoomControl.zoomFactor;
                this.zoomToStageXY(a, this.mouseX, this.mouseY)
            }
        },
        getDevInfo: function() {
            var a = this.zoomLevel(),
                a = {
                    chart: this,
                    type: "writeDevInfo",
                    zoomLevel: a,
                    zoomX: this.zoomX(),
                    zoomY: this.zoomY(),
                    zoomLatitude: this.zoomLatitude(),
                    zoomLongitude: this.zoomLongitude(),
                    latitude: this.coordinateToLatitude((this.mouseY - this.mapContainer.y) / a),
                    longitude: this.coordinateToLongitude((this.mouseX - this.mapContainer.x) / a),
                    left: this.mouseX,
                    top: this.mouseY,
                    right: this.realWidth - this.mouseX,
                    bottom: this.realHeight - this.mouseY,
                    percentLeft: Math.round(this.mouseX / this.realWidth * 100) + "%",
                    percentTop: Math.round(this.mouseY / this.realHeight * 100) + "%",
                    percentRight: Math.round((this.realWidth - this.mouseX) / this.realWidth * 100) + "%",
                    percentBottom: Math.round((this.realHeight - this.mouseY) / this.realHeight * 100) + "%"
                },
                b = "zoomLevel:" + a.zoomLevel + ", zoomLongitude:" + a.zoomLongitude +
                ", zoomLatitude:" + a.zoomLatitude + "\n",
                b = b + ("zoomX:" + a.zoomX + ", zoomY:" + a.zoomY + "\n"),
                b = b + ("latitude:" + a.latitude + ", longitude:" + a.longitude + "\n"),
                b = b + ("left:" + a.left + ", top:" + a.top + "\n"),
                b = b + ("right:" + a.right + ", bottom:" + a.bottom + "\n"),
                b = b + ("left:" + a.percentLeft + ", top:" + a.percentTop + "\n"),
                b = b + ("right:" + a.percentRight + ", bottom:" + a.percentBottom + "\n");
            a.str = b;
            this.fire(a.type, a);
            return a
        },
        getXY: function(a, b, c) {
            void 0 !== a && (-1 != String(a).indexOf("%") ? (a = Number(a.split("%").join("")), c && (a = 100 - a), a = Number(a) * b / 100) : c && (a = b - a));
            return a
        },
        getObjectById: function(a) {
            var b = this.dataProvider;
            if (b.areas) {
                var c = this.getObject(a, b.areas);
                if (c) return c
            }
            if (c = this.getObject(a, b.images)) return c;
            if (a = this.getObject(a, b.lines)) return a
        },
        getObject: function(a, b) {
            if (b) {
                var c;
                for (c = 0; c < b.length; c++) {
                    var d = b[c];
                    if (d.id == a) return d;
                    if (d.areas) {
                        var f = this.getObject(a, d.areas);
                        if (f) return f
                    }
                    if (f = this.getObject(a, d.images)) return f;
                    if (d = this.getObject(a, d.lines)) return d
                }
            }
        },
        parseData: function() {
            var a = this.dataProvider;
            this.processObject(a.areas, a, "area");
            this.processObject(a.images, a, "image");
            this.processObject(a.lines, a, "line")
        },
        processObject: function(a, b, c) {
            if (a) {
                var d;
                for (d = 0; d < a.length; d++) {
                    var f = a[d];
                    f.parentObject = b;
                    "area" == c && e.extend(f, new e.MapArea(this.theme), !0);
                    "image" == c && (f = e.extend(f, new e.MapImage(this.theme), !0));
                    "line" == c && (f = e.extend(f, new e.MapLine(this.theme), !0));
                    a[d] = f;
                    f.areas && this.processObject(f.areas, f, "area");
                    f.images && this.processObject(f.images, f, "image");
                    f.lines && this.processObject(f.lines, f, "line")
                }
            }
        },
        positionChanged: function() {
            var a = {
                type: "positionChanged",
                zoomX: this.zoomX(),
                zoomY: this.zoomY(),
                zoomLevel: this.zoomLevel(),
                chart: this
            };
            this.fire(a.type, a)
        },
        getX: function(a, b) {
            return this.getXY(a, this.realWidth, b)
        },
        getY: function(a, b) {
            return this.getXY(a, this.realHeight, b)
        },
        trim: function(a) {
            if (a) {
                var b;
                for (b = 0; b < a.length; b++)
                    if (-1 === " \n\r\t\f\x0B\u00a0\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u200b\u2028\u2029\u3000".indexOf(a.charAt(b))) {
                        a = a.substring(b);
                        break
                    }
                for (b = a.length - 1; 0 <= b; b--)
                    if (-1 === " \n\r\t\f\x0B\u00a0\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u200b\u2028\u2029\u3000".indexOf(a.charAt(b))) {
                        a = a.substring(0, b + 1);
                        break
                    }
                return -1 === " \n\r\t\f\x0B\u00a0\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u200b\u2028\u2029\u3000".indexOf(a.charAt(0)) ? a : ""
            }
        },
        destroy: function() {
            e.AmMap.base.destroy.call(this)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.ZoomControl = e.Class({
        construct: function(a) {
            this.cname = "ZoomControl";
            this.panStepSize = .1;
            this.zoomFactor = 2;
            this.maxZoomLevel = 64;
            this.minZoomLevel = 1;
            this.panControlEnabled = !1;
            this.zoomControlEnabled = !0;
            this.buttonRollOverColor = "#DADADA";
            this.buttonFillColor = "#FFFFFF";
            this.buttonFillAlpha = 1;
            this.buttonBorderColor = "#000000";
            this.buttonBorderAlpha = .1;
            this.buttonIconAlpha = this.buttonBorderThickness = 1;
            this.gridColor = this.buttonIconColor = "#000000";
            this.homeIconFile = "homeIcon.gif";
            this.gridBackgroundColor = "#000000";
            this.draggerAlpha = this.gridAlpha = this.gridBackgroundAlpha = 0;
            this.draggerSize = this.buttonSize = 31;
            this.iconSize = 11;
            this.homeButtonEnabled = !0;
            this.buttonCornerRadius = 2;
            this.gridHeight = 5;
            this.roundButtons = !0;
            this.top = this.left = 10;
            e.applyTheme(this, a, this.cname)
        },
        init: function(a, b) {
            var c = this;
            c.chart = a;
            e.remove(c.set);
            var d = b.set();
            e.setCN(a, d, "zoom-control");
            var f = c.buttonSize,
                k = c.zoomControlEnabled,
                g = c.panControlEnabled,
                h = c.buttonFillColor,
                l = c.buttonFillAlpha,
                n = c.buttonBorderThickness,
                m = c.buttonBorderColor,
                r = c.buttonBorderAlpha,
                u = c.buttonCornerRadius,
                q = c.buttonRollOverColor,
                p = c.gridHeight,
                w = c.zoomFactor,
                E = c.minZoomLevel,
                z = c.maxZoomLevel,
                C = c.buttonIconAlpha,
                x = c.buttonIconColor,
                D = c.roundButtons,
                F = a.svgIcons,
                t = a.getX(c.left),
                v = a.getY(c.top);
            isNaN(c.right) || (t = a.getX(c.right, !0), t = g ? t - 3 * f : t - f);
            isNaN(c.bottom) || (v = a.getY(c.bottom, !0), k && (v -= p + 3 * f), v = g ? v - 3 * f : c.homeButtonEnabled ? v - .5 * f : v + f);
            d.translate(t, v);
            c.previousDY = NaN;
            var B, t = f / 4 - 1;
            if (k) {
                B = b.set();
                e.setCN(a, B, "zoom-control-zoom");
                d.push(B);
                c.set = d;
                c.zoomSet = B;
                k = e.rect(b, f + 6, p + 2 * f + 6, c.gridBackgroundColor, c.gridBackgroundAlpha, 0, 0, 0, 4);
                e.setCN(a, k, "zoom-bg");
                k.translate(-3, -3);
                k.mouseup(function() {
                    c.handleBgUp()
                }).touchend(function() {
                    c.handleBgUp()
                });
                B.push(k);
                var A = f;
                D && (A = f / 1.5);
                c.draggerSize = A;
                var H = Math.log(z / E) / Math.log(w) + 1;
                1E3 < H && (H = 1E3);
                var k = p / H,
                    y, J = b.set();
                J.translate((f - A) / 2 + 1, 1, NaN, !0);
                B.push(J);
                for (y = 1; y < H; y++) v = f + y * k, v = e.line(b, [1, A - 2], [v, v], c.gridColor, c.gridAlpha, 1), e.setCN(a, v, "zoom-grid"), J.push(v);
                v = new e.SimpleButton;
                v.setDownHandler(c.draggerDown, c);
                v.setClickHandler(c.draggerUp, c);
                v.init(b, A, k, h, l, n, m, r, u, q);
                e.setCN(a, v.set, "zoom-dragger");
                B.push(v.set);
                v.set.setAttr("opacity", c.draggerAlpha);
                c.dragger = v.set;
                c.previousY = NaN;
                v = new e.SimpleButton;
                F ? (A = b.set(), H = e.line(b, [-t, t], [0, 0], x, C, 1), y = e.line(b, [0, 0], [-t, t], x, C, 1), A.push(H), A.push(y), v.svgIcon = A) : v.setIcon(a.pathToImages + "plus.gif", c.iconSize);
                v.setClickHandler(a.zoomIn, a);
                v.init(b, f, f, h, l, n, m, r, u, q, C, x, D);
                e.setCN(a, v.set, "zoom-in");
                B.push(v.set);
                v = new e.SimpleButton;
                F ? v.svgIcon = e.line(b, [-t, t], [0, 0], x, C, 1) : v.setIcon(a.pathToImages + "minus.gif", c.iconSize);
                v.setClickHandler(a.zoomOut, a);
                v.init(b, f, f, h, l, n, m, r, u, q, C, x, D);
                v.set.translate(0, p + f);
                e.setCN(a, v.set, "zoom-out");
                B.push(v.set);
                p -= k;
                z = Math.log(z / 100) / Math.log(w);
                c.realStepSize = p / (z - Math.log(E / 100) / Math.log(w));
                c.realGridHeight = p;
                c.stepMax = z
            }
            g && (g = b.set(), e.setCN(a, g, "zoom-control-pan"), d.push(g), B && B.translate(f, 4 * f), w = new e.SimpleButton, F ? w.svgIcon = e.line(b, [t / 5, -t + t / 5, t / 5], [-t, 0, t], x, C, 1) : w.setIcon(a.pathToImages +
                "panLeft.gif", c.iconSize), w.setClickHandler(a.moveLeft, a), w.init(b, f, f, h, l, n, m, r, u, q, C, x, D), w.set.translate(0, f), e.setCN(a, w.set, "pan-left"), g.push(w.set), w = new e.SimpleButton, F ? w.svgIcon = e.line(b, [-t / 5, t - t / 5, -t / 5], [-t, 0, t], x, C, 1) : w.setIcon(a.pathToImages + "panRight.gif", c.iconSize), w.setClickHandler(a.moveRight, a), w.init(b, f, f, h, l, n, m, r, u, q, C, x, D), w.set.translate(2 * f, f), e.setCN(a, w.set, "pan-right"), g.push(w.set), w = new e.SimpleButton, F ? w.svgIcon = e.line(b, [-t, 0, t], [t / 5, -t + t / 5, t / 5], x, C, 1) : w.setIcon(a.pathToImages + "panUp.gif", c.iconSize), w.setClickHandler(a.moveUp, a), w.init(b, f, f, h, l, n, m, r, u, q, C, x, D), w.set.translate(f, 0), e.setCN(a, w.set, "pan-up"), g.push(w.set), w = new e.SimpleButton, F ? w.svgIcon = e.line(b, [-t, 0, t], [-t / 5, t - t / 5, -t / 5], x, C, 1) : w.setIcon(a.pathToImages + "panDown.gif", c.iconSize), w.setClickHandler(a.moveDown, a), w.init(b, f, f, h, l, n, m, r, u, q, C, x, D), w.set.translate(f, 2 * f), e.setCN(a, w.set, "pan-down"), g.push(w.set), d.push(g));
            c.homeButtonEnabled && (g = new e.SimpleButton, F ? g.svgIcon = e.polygon(b, [-t, 0, t, t - 1, t - 1, 2, 2, -2, -2, -t + 1, -t + 1], [0, -t, 0, 0, t - 1, t - 1, 2, 2, t - 1, t - 1, 0], x, C, 1, x, C) : g.setIcon(a.pathToImages + c.homeIconFile, c.iconSize), g.setClickHandler(a.goHome, a), c.panControlEnabled && (r = l = 0), g.init(b, f, f, h, l, n, m, r, u, q, C, x, D), c.panControlEnabled ? g.set.translate(f, f) : B && B.translate(0, 1.5 * f), e.setCN(a, g.set, "pan-home"), d.push(g.set));
            c.update()
        },
        draggerDown: function() {
            this.chart.stopDrag();
            this.isDragging = !0
        },
        draggerUp: function() {
            this.isDragging = !1
        },
        handleBgUp: function() {
            var a = this.chart;
            a.zoomTo(100 * Math.pow(this.zoomFactor, this.stepMax - (a.mouseY - this.zoomSet.y - this.set.y - this.buttonSize - this.realStepSize / 2) / this.realStepSize))
        },
        update: function() {
            var a;
            a = this.zoomFactor;
            var b = this.realStepSize,
                c = this.stepMax,
                d = this.dragger,
                f = this.buttonSize,
                k, g = this.chart;
            g && (this.isDragging ? (g.stopDrag(), k = d.y + (g.mouseY - this.previousY), k = e.fitToBounds(k, f, this.realGridHeight + f), g.zoomTo(100 * Math.pow(a, c - (k - f) / b), NaN, NaN, !0)) : (a = Math.log(g.zoomLevel() / 100) / Math.log(a), k = (c - a) * b + f), this.previousY = g.mouseY, this.previousDY != k && d && (d.translate((this.buttonSize -
                this.draggerSize) / 2, k), this.previousDY = k))
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.SimpleButton = e.Class({
        construct: function() {},
        init: function(a, b, c, d, f, k, g, h, l, n, m, r, u) {
            var q = this;
            q.rollOverColor = n;
            q.color = d;
            q.container = a;
            n = a.set();
            q.set = n;
            u ? (b /= 2, d = e.circle(a, b, d, f, k, g, h), d.translate(b, b)) : d = e.rect(a, b, c, d, f, k, g, h, l);
            n.push(d);
            f = q.iconPath;
            var p;
            f && (p = q.iconSize, k = (b - p) / 2, u && (k = (2 * b - p) / 2), p = a.image(f, k, (c - p) / 2, p, p));
            q.svgIcon && (p = q.svgIcon, u ? p.translate(b, b) : p.translate(b / 2, b / 2));
            n.setAttr("cursor", "pointer");
            p && (n.push(p), p.setAttr("opacity",
                m), p.node.style.pointerEvents = "none");
            d.mousedown(function() {
                q.handleDown()
            }).touchstart(function() {
                q.handleDown()
            }).mouseup(function() {
                q.handleUp()
            }).touchend(function() {
                q.handleUp()
            }).mouseover(function() {
                q.handleOver()
            }).mouseout(function() {
                q.handleOut()
            });
            q.bg = d
        },
        setIcon: function(a, b) {
            this.iconPath = a;
            this.iconSize = b
        },
        setClickHandler: function(a, b) {
            this.clickHandler = a;
            this.scope = b
        },
        setDownHandler: function(a, b) {
            this.downHandler = a;
            this.scope = b
        },
        handleUp: function() {
            var a = this.clickHandler;
            a && a.call(this.scope)
        },
        handleDown: function() {
            var a = this.downHandler;
            a && a.call(this.scope)
        },
        handleOver: function() {
            this.container.chart.skipClick = !0;
            this.bg.setAttr("fill", this.rollOverColor)
        },
        handleOut: function() {
            this.container.chart.skipClick = !1;
            this.bg.setAttr("fill", this.color)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.SmallMap = e.Class({
        construct: function(a) {
            this.cname = "SmallMap";
            this.mapColor = "#e6e6e6";
            this.rectangleColor = "#FFFFFF";
            this.top = this.right = 10;
            this.minimizeButtonWidth = 23;
            this.backgroundColor = "#9A9A9A";
            this.backgroundAlpha = 1;
            this.borderColor = "#FFFFFF";
            this.iconColor = "#000000";
            this.borderThickness = 3;
            this.borderAlpha = 1;
            this.size = .2;
            this.enabled = !0;
            e.applyTheme(this, a, this.cname)
        },
        init: function(a, b) {
            var c = this;
            if (c.enabled) {
                c.chart = a;
                c.container = b;
                c.width = a.realWidth * c.size;
                c.height = a.realHeight * c.size;
                e.remove(c.set);
                var d = b.set();
                c.set = d;
                e.setCN(a, d, "small-map");
                var f = b.set();
                c.allSet = f;
                d.push(f);
                c.buildSVGMap();
                var k = c.borderThickness,
                    g = c.borderColor,
                    h = e.rect(b, c.width + k, c.height + k, c.backgroundColor, c.backgroundAlpha, k, g, c.borderAlpha);
                e.setCN(a, h, "small-map-bg");
                h.translate(-k / 2, -k / 2);
                f.push(h);
                h.toBack();
                var l, n, h = c.minimizeButtonWidth,
                    m = new e.SimpleButton,
                    r = h / 2;
                a.svgIcons ? m.svgIcon = e.line(b, [-r / 2, 0, r / 2], [-r / 4, r / 4, -r / 4], c.iconColor, 1, 1) : m.setIcon(a.pathToImages + "arrowDown.gif", h);
                m.setClickHandler(c.minimize, c);
                m.init(b, h, h, g, 1, 1, g, 1);
                e.setCN(a, m.set, "small-map-down");
                m = m.set;
                c.downButtonSet = m;
                d.push(m);
                var u = new e.SimpleButton;
                a.svgIcons ? u.svgIcon = e.line(b, [-r / 2, 0, r / 2], [r / 4, -r / 4, r / 4], c.iconColor, 1, 1) : u.setIcon(a.pathToImages + "arrowUp.gif", h);
                u.setClickHandler(c.maximize, c);
                u.init(b, h, h, g, 1, 1, g, 1);
                e.setCN(a, u.set, "small-map-up");
                g = u.set;
                c.upButtonSet = g;
                g.hide();
                d.push(g);
                var q, p;
                isNaN(c.top) || (l = a.getY(c.top) + k, p = 0);
                isNaN(c.bottom) || (l = a.getY(c.bottom, !0) -
                    c.height - k, p = c.height - h + k / 2);
                isNaN(c.left) || (n = a.getX(c.left) + k, q = -k / 2);
                isNaN(c.right) || (n = a.getX(c.right, !0) - c.width - k, q = c.width - h + k / 2);
                k = b.set();
                k.clipRect(1, 1, c.width, c.height);
                f.push(k);
                c.rectangleC = k;
                d.translate(n, l);
                m.translate(q, p);
                g.translate(q, p);
                f.mouseup(function() {
                    c.handleMouseUp()
                });
                c.drawRectangle()
            } else e.remove(c.allSet), e.remove(c.downButtonSet), e.remove(c.upButtonSet)
        },
        minimize: function() {
            this.downButtonSet.hide();
            this.upButtonSet.show();
            this.allSet.hide()
        },
        maximize: function() {
            this.downButtonSet.show();
            this.upButtonSet.hide();
            this.allSet.show()
        },
        buildSVGMap: function() {
            var a = this.chart,
                b = {
                    fill: this.mapColor,
                    stroke: this.mapColor,
                    "stroke-opacity": 1
                },
                c = a.svgData.g.path,
                d = this.container,
                f = d.set();
            e.setCN(a, f, "small-map-image");
            var k;
            for (k = 0; k < c.length; k++) {
                var g = d.path(c[k].d).attr(b);
                f.push(g)
            }
            this.allSet.push(f);
            b = f.getBBox();
            c = this.size * a.mapScale;
            d = -b.x * c;
            k = -b.y * c;
            var h = g = 0;
            a.centerMap && (g = (this.width - b.width * c) / 2, h = (this.height - b.height * c) / 2);
            this.mapWidth = b.width * c;
            this.mapHeight = b.height * c;
            this.dx =
                g;
            this.dy = h;
            f.translate(d + g, k + h, c)
        },
        update: function() {
            var a = this.chart;
            if (a) {
                var b = a.zoomLevel(),
                    c = this.width,
                    d = a.mapContainer,
                    a = c / (a.realWidth * b),
                    c = c / b,
                    b = this.height / b,
                    f = this.rectangle;
                f.translate(-d.x * a + this.dx, -d.y * a + this.dy);
                0 < c && 0 < b && (f.setAttr("width", Math.ceil(c + 1)), f.setAttr("height", Math.ceil(b + 1)));
                this.rWidth = c;
                this.rHeight = b
            }
        },
        drawRectangle: function() {
            var a = this.rectangle;
            e.remove(a);
            a = e.rect(this.container, 10, 10, "#000", 0, 1, this.rectangleColor, 1);
            e.setCN(this.chart, a, "small-map-rectangle");
            this.rectangleC.push(a);
            this.rectangle = a
        },
        handleMouseUp: function() {
            var a = this.chart,
                b = a.zoomLevel();
            a.zoomTo(b, -((a.mouseX - this.set.x - this.dx - this.rWidth / 2) / this.mapWidth) * b, -((a.mouseY - this.set.y - this.dy - this.rHeight / 2) / this.mapHeight) * b)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.AreasProcessor = e.Class({
        construct: function(a) {
            this.chart = a
        },
        process: function(a) {
            this.updateAllAreas();
            this.allObjects = [];
            a = a.areas;
            var b = this.chart,
                c = a.length,
                d, f, e = 0,
                g = !1,
                h = !1,
                l = 0;
            for (d = 0; d < c; d++)
                if (f = a[d], f = f.value, !isNaN(f)) {
                    if (!1 === g || g < f) g = f;
                    if (!1 === h || h > f) h = f;
                    e += Math.abs(f);
                    l++
                }
            this.minValue = h;
            this.maxValue = g;
            isNaN(b.minValue) || (this.minValue = b.minValue);
            isNaN(b.maxValue) || (this.maxValue = b.maxValue);
            b.maxValueReal = g;
            b.minValueReal = h;
            for (d = 0; d < c; d++) f = a[d],
                isNaN(f.value) ? f.percents = void 0 : (f.percents = (f.value - h) / e * 100, h == g && (f.percents = 100));
            for (d = 0; d < c; d++) f = a[d], this.createArea(f)
        },
        updateAllAreas: function() {
            var a = this.chart,
                b = a.areasSettings,
                c = b.unlistedAreasColor,
                d = b.unlistedAreasAlpha,
                f = b.unlistedAreasOutlineColor,
                k = b.unlistedAreasOutlineAlpha,
                g = a.svgAreas,
                h = a.dataProvider,
                l = h.areas,
                n = {},
                m;
            for (m = 0; m < l.length; m++) n[l[m].id] = l[m];
            for (m = 0; m < g.length; m++) {
                l = g[m];
                if (b.preserveOriginalAttributes) {
                    if (l.customAttr)
                        for (var r in l.customAttr) l.setAttr(r,
                            l.customAttr[r])
                } else {
                    void 0 != c && l.setAttr("fill", c);
                    isNaN(d) || l.setAttr("fill-opacity", d);
                    void 0 != f && l.setAttr("stroke", f);
                    isNaN(k) || l.setAttr("stroke-opacity", k);
                    var u = b.outlineThickness;
                    b.adjustOutlineThickness && (u /= a.zoomLevel());
                    l.setAttr("stroke-width", u)
                }
                e.setCN(a, l, "map-area-unlisted");
                h.getAreasFromMap && !n[l.id] && (u = new e.MapArea(a.theme), u.parentObject = h, u.id = l.id, h.areas.push(u))
            }
        },
        createArea: function(a) {
            var b = this.chart,
                c = b.svgAreasById[a.id],
                d = b.areasSettings;
            if (c && c.className) {
                var f = b.areasClasses[c.className];
                f && (d = e.processObject(f, e.AreasSettings, b.theme))
            }
            var k = d.color,
                g = d.alpha,
                h = d.outlineThickness,
                l = d.rollOverColor,
                n = d.selectedColor,
                m = d.rollOverAlpha,
                r = d.rollOverBrightness,
                u = d.outlineColor,
                q = d.outlineAlpha,
                p = d.balloonText,
                w = d.selectable,
                E = d.pattern,
                z = d.rollOverOutlineColor,
                C = d.bringForwardOnHover,
                x = d.preserveOriginalAttributes;
            this.allObjects.push(a);
            a.chart = b;
            a.baseSettings = d;
            a.autoZoomReal = void 0 == a.autoZoom ? d.autoZoom : a.autoZoom;
            f = a.color;
            void 0 == f && (f = k);
            var D = a.alpha;
            isNaN(D) && (D = g);
            g = a.rollOverAlpha;
            isNaN(g) && (g = m);
            isNaN(g) && (g = D);
            m = a.rollOverColor;
            void 0 == m && (m = l);
            l = a.pattern;
            void 0 == l && (l = E);
            E = a.selectedColor;
            void 0 == E && (E = n);
            n = a.balloonText;
            void 0 === n && (n = p);
            void 0 == d.colorSolid || isNaN(a.value) || (p = Math.floor((a.value - this.minValue) / ((this.maxValue - this.minValue) / b.colorSteps)), p == b.colorSteps && p--, p *= 1 / (b.colorSteps - 1), this.maxValue == this.minValue && (p = 1), a.colorReal = e.getColorFade(f, d.colorSolid, p));
            void 0 != a.color && (a.colorReal = a.color);
            void 0 == a.selectable &&
                (a.selectable = w);
            void 0 == a.colorReal && (a.colorReal = k);
            k = a.outlineColor;
            void 0 == k && (k = u);
            u = a.outlineAlpha;
            isNaN(u) && (u = q);
            q = a.outlineThickness;
            isNaN(q) && (q = h);
            h = a.rollOverOutlineColor;
            void 0 == h && (h = z);
            z = a.rollOverBrightness;
            void 0 == z && (z = r);
            void 0 == a.bringForwardOnHover && (a.bringForwardOnHover = C);
            void 0 == a.preserveOriginalAttributes && (a.preserveOriginalAttributes = x);
            isNaN(d.selectedBrightness) || (E = e.adjustLuminosity(a.colorReal, d.selectedBrightness / 100));
            a.alphaReal = D;
            a.rollOverColorReal = m;
            a.rollOverAlphaReal = g;
            a.balloonTextReal = n;
            a.selectedColorReal = E;
            a.outlineColorReal = k;
            a.outlineAlphaReal = u;
            a.rollOverOutlineColorReal = h;
            a.outlineThicknessReal = q;
            a.patternReal = l;
            a.rollOverBrightnessReal = z;
            e.processDescriptionWindow(d, a);
            if (c && (r = c.area, C = c.title, a.enTitle = c.title, C && !a.title && (a.title = C), (c = b.language) ? (C = e.mapTranslations) && (c = C[c]) && c[a.enTitle] && (a.titleTr = c[a.enTitle]) : a.titleTr = void 0, r)) {
                a.displayObject = r;
                a.mouseEnabled && b.addObjectEventListeners(r, a);
                var F;
                void 0 != f && (F = f);
                void 0 != a.colorReal && (F = a.showAsSelected || b.selectedObject == a ? a.selectedColorReal : a.colorReal);
                r.node.setAttribute("class", "");
                e.setCN(b, r, "map-area");
                e.setCN(b, r, "map-area-" + r.id);
                d.adjustOutlineThickness && (q /= b.zoomLevel());
                a.preserveOriginalAttributes || (r.setAttr("fill", F), r.setAttr("stroke", k), r.setAttr("stroke-opacity", u), r.setAttr("stroke-width", q), r.setAttr("fill-opacity", D));
                l && r.pattern(l, b.mapScale, b.path);
                a.hidden && r.hide()
            }
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.AreasSettings = e.Class({
        construct: function(a) {
            this.cname = "AreasSettings";
            this.alpha = 1;
            this.autoZoom = !1;
            this.balloonText = "[[title]]";
            this.color = "#FFCC00";
            this.colorSolid = "#990000";
            this.unlistedAreasAlpha = 1;
            this.unlistedAreasColor = "#DDDDDD";
            this.outlineColor = "#FFFFFF";
            this.outlineThickness = this.outlineAlpha = 1;
            this.selectedColor = this.rollOverOutlineColor = "#CC0000";
            this.unlistedAreasOutlineColor = "#FFFFFF";
            this.unlistedAreasOutlineAlpha = 1;
            this.descriptionWindowWidth = 250;
            this.bringForwardOnHover = this.adjustOutlineThickness = !0;
            e.applyTheme(this, a, this.cname)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.ImagesProcessor = e.Class({
        construct: function(a) {
            this.chart = a;
            this.reset()
        },
        process: function(a) {
            var b = a.images,
                c;
            for (c = 0; c < b.length; c++) {
                var d = b[c];
                this.createImage(d, c);
                d.parentArray = b
            }
            this.counter = c;
            a.parentObject && a.remainVisible && this.process(a.parentObject)
        },
        createImage: function(a, b) {
            a = e.processObject(a, e.MapImage);
            isNaN(b) && (this.counter++, b = this.counter);
            var c = this.chart,
                d = c.container,
                f = c.mapImagesContainer,
                k = c.stageImagesContainer,
                g = c.imagesSettings;
            a.remove && a.remove();
            var h = g.color,
                l = g.alpha,
                n = g.rollOverColor,
                m = g.rollOverOutlineColor,
                r = g.selectedColor,
                u = g.balloonText,
                q = g.outlineColor,
                p = g.outlineAlpha,
                w = g.outlineThickness,
                E = g.selectedScale,
                z = g.rollOverScale,
                C = g.labelPosition,
                x = g.labelColor,
                D = g.labelFontSize,
                F = g.bringForwardOnHover,
                t = g.labelRollOverColor,
                v = g.rollOverBrightness,
                B = g.selectedLabelColor;
            a.index = b;
            a.chart = c;
            a.baseSettings = c.imagesSettings;
            var A = d.set();
            a.displayObject = A;
            var H = a.color;
            void 0 == H && (H = h);
            h = a.alpha;
            isNaN(h) && (h = l);
            void 0 == a.bringForwardOnHover && (a.bringForwardOnHover = F);
            l = a.outlineAlpha;
            isNaN(l) && (l = p);
            p = a.rollOverColor;
            void 0 == p && (p = n);
            n = a.selectedColor;
            void 0 == n && (n = r);
            r = a.balloonText;
            void 0 === r && (r = u);
            u = a.outlineColor;
            void 0 == u && (u = q);
            a.outlineColorReal = u;
            q = a.outlineThickness;
            isNaN(q) && (q = w);
            (w = a.labelPosition) || (w = C);
            C = a.labelColor;
            void 0 == C && (C = x);
            x = a.labelRollOverColor;
            void 0 == x && (x = t);
            t = a.selectedLabelColor;
            void 0 == t && (t = B);
            B = a.labelFontSize;
            isNaN(B) && (B = D);
            D = a.selectedScale;
            isNaN(D) && (D = E);
            E = a.rollOverScale;
            isNaN(E) && (E = z);
            z = a.rollOverBrightness;
            void 0 == z && (z = v);
            a.colorReal = H;
            isNaN(g.selectedBrightness) || (n = e.adjustLuminosity(a.colorReal, g.selectedBrightness / 100));
            a.alphaReal = h;
            a.rollOverColorReal = p;
            a.balloonTextReal = r;
            a.selectedColorReal = n;
            a.labelColorReal = C;
            a.labelRollOverColorReal = x;
            a.selectedLabelColorReal = t;
            a.labelFontSizeReal = B;
            a.labelPositionReal = w;
            a.selectedScaleReal = D;
            a.rollOverScaleReal = E;
            a.rollOverOutlineColorReal = m;
            a.rollOverBrightnessReal = z;
            e.processDescriptionWindow(g, a);
            a.centeredReal = void 0 == a.centered ? g.centered : a.centered;
            z = a.type;
            E = a.imageURL;
            D = a.svgPath;
            B = a.width;
            p = a.height;
            m = a.scale;
            isNaN(a.percentWidth) || (B = a.percentWidth / 100 * c.realWidth);
            isNaN(a.percentHeight) || (p = a.percentHeight / 100 * c.realHeight);
            var y;
            E || z || D || (z = "circle", B = 1, l = h = 0);
            t = v = 0;
            g = a.selectedColorReal;
            if (z) {
                isNaN(B) && (B = 10);
                isNaN(p) && (p = 10);
                "kilometers" == a.widthAndHeightUnits && (B = c.kilometersToPixels(a.width), p = c.kilometersToPixels(a.height));
                "miles" == a.widthAndHeightUnits && (B = c.milesToPixels(a.width), p = c.milesToPixels(a.height));
                if ("circle" == z || "bubble" == z) p = B;
                y = this.createPredefinedImage(H, u, q, z, B, p);
                t = v = 0;
                a.centeredReal ? (isNaN(a.right) || (v = B * m), isNaN(a.bottom) || (t = p * m)) : (v = B * m / 2, t = p * m / 2);
                y.translate(v, t, m, !0)
            } else E ? (isNaN(B) && (B = 10), isNaN(p) && (p = 10), y = d.image(E, 0, 0, B, p), y.node.setAttribute("preserveAspectRatio", "none"), y.setAttr("opacity", h), a.centeredReal && (v = isNaN(a.right) ? -B / 2 : B / 2, t = isNaN(a.bottom) ? -p / 2 : p / 2, y.translate(v, t, NaN, !0))) : D && (y = d.path(D), z = y.getBBox(), a.centeredReal ? (v = -z.x * m - z.width * m / 2, isNaN(a.right) || (v = -v), t = -z.y * m - z.height * m /
                2, isNaN(a.bottom) || (t = -t)) : v = t = 0, y.translate(v, t, m, !0), y.x = v, y.y = t);
            y && (A.push(y), a.image = y, y.setAttr("stroke-opacity", l), y.setAttr("stroke-width", q), y.setAttr("stroke", u), y.setAttr("fill-opacity", h), y.setAttr("fill", H), e.setCN(c, y, "map-image"), void 0 != a.id && e.setCN(c, y, "map-image-" + a.id));
            H = a.labelColorReal;
            !a.showAsSelected && c.selectedObject != a || void 0 == g || (y.setAttr("fill", g), H = a.selectedLabelColorReal);
            y = null;
            void 0 !== a.label && (y = e.text(d, a.label, H, c.fontFamily, a.labelFontSizeReal, a.labelAlign), e.setCN(c, y, "map-image-label"), void 0 !== a.id && e.setCN(c, y, "map-image-label-" + a.id), H = a.labelBackgroundAlpha, (h = a.labelBackgroundColor) && 0 < H && (l = y.getBBox(), d = e.rect(d, l.width + 16, l.height + 10, h, H), e.setCN(c, d, "map-image-label-background"), void 0 != a.id && e.setCN(c, d, "map-image-label-background-" + a.id), A.push(d), a.labelBG = d), a.imageLabel = y, A.push(y), e.setCN(c, A, "map-image-container"), void 0 != a.id && e.setCN(c, A, "map-image-container-" + a.id));
            d = isNaN(a.latitude) || isNaN(a.longitude) ? !0 : !1;
            a.lineId && (y = this.chart.getObjectById(a.lineId)) && 0 < y.longitudes.length && (d = !1);
            d ? k.push(A) : f.push(A);
            A && (A.rotation = a.rotation, isNaN(a.rotation) || A.rotate(a.rotation));
            this.updateSizeAndPosition(a);
            a.mouseEnabled && c.addObjectEventListeners(A, a);
            a.hidden && A.hide();
            a.animateAlongLine && setTimeout(function() {
                a.animateAlong.call(a)
            }, 100);
            return a
        },
        updateSizeAndPosition: function(a) {
            var b = this.chart,
                c = a.displayObject,
                d = b.getX(a.left),
                f = b.getY(a.top),
                k, g = a.image.getBBox();
            isNaN(a.right) || (d = b.getX(a.right, !0) - g.width * a.scale);
            isNaN(a.bottom) || (f = b.getY(a.bottom, !0) - g.height * a.scale);
            var h = a.longitude,
                l = a.latitude,
                n = a.positionOnLine,
                g = this.objectsToResize;
            this.allSvgObjects.push(c);
            this.allObjects.push(a);
            a.arrays.push({
                arr: this.allSvgObjects,
                el: c
            });
            a.arrays.push({
                arr: this.allObjects,
                el: a
            });
            var m = a.imageLabel,
                r = this.chart.zoomLevel();
            if (a.lineId) {
                var u = this.chart.getObjectById(a.lineId);
                (a.line = u) && u.getCoordinates && (u.chart = b, u = u.getCoordinates(n, a.lineSegment)) && (h = b.coordinateToLongitude(u.x), l = b.coordinateToLatitude(u.y), k = e.radiansToDegrees(u.angle))
            }
            isNaN(k) || c.rotate(k + a.extraAngle);
            if (!isNaN(d) && !isNaN(f)) c.translate(d, f, NaN, !0);
            else if (!isNaN(l) && !isNaN(h))
                if (d = b.longitudeToCoordinate(h), f = b.latitudeToCoordinate(l), a.fixedSize) {
                    k = 1;
                    if (a.showAsSelected || b.selectedObject == a) k = a.selectedScaleReal;
                    b = a.positionScale;
                    isNaN(b) ? b = 0 : (--b, b *= 1 - 2 * Math.abs(n - .5));
                    n = {
                        image: c,
                        scale: k + b
                    };
                    g.push(n);
                    a.arrays.push({
                        arr: g,
                        el: n
                    });
                    c.translate(d, f, k / r + b, !0)
                } else c.translate(d, f, NaN, !0), m && (this.labelsToReposition.push(a), a.arrays.push({
                    arr: this.labelsToReposition,
                    el: a
                }));
            this.positionLabel(m, a, a.labelPositionReal)
        },
        positionLabel: function(a, b, c) {
            if (a) {
                var d = b.image,
                    f = 0,
                    e = 0,
                    g = 0,
                    h = 0;
                d && (h = d.getBBox(), e = d.y, f = d.x, g = h.width, h = h.height, b.svgPath && (g *= b.scale, h *= b.scale));
                var d = a.getBBox(),
                    l = d.width,
                    n = d.height;
                "right" == c && (f += g + l / 2 + 5, e += h / 2 - 2);
                "left" == c && (f += -l / 2 - 5, e += h / 2 - 2);
                "top" == c && (e -= n / 2 + 3, f += g / 2);
                "bottom" == c && (e += h + n / 2, f += g / 2);
                "middle" == c && (f += g / 2, e += h / 2);
                a.translate(f + b.labelShiftX, e + b.labelShiftY, NaN, !0);
                b.labelBG && b.labelBG.translate(f - d.width / 2 + b.labelShiftX - 9, e + b.labelShiftY - d.height / 2 - 3, NaN, !0)
            }
        },
        createPredefinedImage: function(a, b, c, d, f, k) {
            var g = this.chart.container,
                h;
            switch (d) {
                case "circle":
                    h = e.circle(g, f / 2, a, 1, c, b, 1);
                    break;
                case "rectangle":
                    h = e.polygon(g, [-f / 2, f / 2, f / 2, -f / 2], [k / 2, k / 2, -k / 2, -k / 2], a, 1, c, b, 1, 0, !0);
                    break;
                case "bubble":
                    h = e.circle(g, f / 2, a, 1, c, b, 1, !0);
                    break;
                case "hexagon":
                    f /= Math.sqrt(3), h = e.polygon(g, [.866 * f, 0 * f, -.866 * f, -.866 * f, 0 * f, .866 * f], [.5 * f, 1 * f, .5 * f, -.5 * f, -1 * f, -.5 * f], a, 1, c, b, 1)
            }
            return h
        },
        reset: function() {
            this.objectsToResize = [];
            this.allSvgObjects = [];
            this.allObjects = [];
            this.allLabels = [];
            this.labelsToReposition = []
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.ImagesSettings = e.Class({
        construct: function(a) {
            this.cname = "ImagesSettings";
            this.balloonText = "[[title]]";
            this.alpha = 1;
            this.borderAlpha = 0;
            this.borderThickness = 1;
            this.labelPosition = "right";
            this.labelColor = "#000000";
            this.labelFontSize = 11;
            this.color = "#000000";
            this.labelRollOverColor = "#00CC00";
            this.centered = !0;
            this.rollOverScale = this.selectedScale = 1;
            this.descriptionWindowWidth = 250;
            this.bringForwardOnHover = !0;
            this.outlineColor = "transparent";
            this.adjustAnimationSpeed = !1;
            this.baseAnimationDistance = 500;
            this.pauseDuration = 0;
            this.easingFunction = e.easeInOutQuad;
            this.animationDuration = 3;
            this.positionScale = 1;
            e.applyTheme(this, a, this.cname)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.LinesProcessor = e.Class({
        construct: function(a) {
            this.chart = a;
            this.reset()
        },
        process: function(a) {
            var b = a.lines,
                c;
            for (c = 0; c < b.length; c++) {
                var d = b[c];
                this.createLine(d, c);
                d.parentArray = b
            }
            this.counter = c;
            a.parentObject && a.remainVisible && this.process(a.parentObject)
        },
        createLine: function(a, b) {
            a = e.processObject(a, e.MapLine);
            isNaN(b) && (this.counter++, b = this.counter);
            a.index = b;
            a.remove && a.remove();
            var c = this.chart,
                d = c.linesSettings,
                f = this.objectsToResize,
                k = c.mapLinesContainer,
                g = c.stageLinesContainer,
                h = d.thickness,
                l = d.dashLength,
                n = d.arrow,
                m = d.arrowSize,
                r = d.arrowColor,
                u = d.arrowAlpha,
                q = d.color,
                p = d.alpha,
                w = d.rollOverColor,
                E = d.selectedColor,
                z = d.rollOverAlpha,
                C = d.balloonText,
                x = d.bringForwardOnHover,
                D = d.arc,
                F = d.rollOverBrightness,
                t = c.container;
            a.chart = c;
            a.baseSettings = d;
            var v = t.set();
            a.displayObject = v;
            this.allSvgObjects.push(v);
            a.arrays.push({
                arr: this.allSvgObjects,
                el: v
            });
            this.allObjects.push(a);
            a.arrays.push({
                arr: this.allObjects,
                el: a
            });
            a.mouseEnabled && c.addObjectEventListeners(v, a);
            if (a.remainVisible || c.selectedObject == a.parentObject) {
                var B = a.thickness;
                isNaN(B) && (B = h);
                h = a.dashLength;
                isNaN(h) && (h = l);
                l = a.color;
                void 0 == l && (l = q);
                q = a.alpha;
                isNaN(q) && (q = p);
                p = a.rollOverAlpha;
                isNaN(p) && (p = z);
                isNaN(p) && (p = q);
                z = a.rollOverColor;
                void 0 == z && (z = w);
                w = a.selectedColor;
                void 0 == w && (w = E);
                E = a.balloonText;
                void 0 === E && (E = C);
                C = a.arc;
                isNaN(C) && (C = D);
                D = a.arrow;
                if (!D || "none" == D && "none" != n) D = n;
                n = a.arrowColor;
                void 0 == n && (n = r);
                void 0 == n && (n = l);
                r = a.arrowAlpha;
                isNaN(r) && (r = u);
                isNaN(r) && (r = q);
                u = a.arrowSize;
                isNaN(u) && (u = m);
                m = a.rollOverBrightness;
                void 0 == m && (m = F);
                a.colorReal = l;
                isNaN(d.selectedBrightness) || (w = e.adjustLuminosity(a.colorReal, d.selectedBrightness / 100));
                a.alphaReal = q;
                a.rollOverColorReal = z;
                a.rollOverAlphaReal = p;
                a.balloonTextReal = E;
                a.selectedColorReal = w;
                a.thicknessReal = B;
                a.rollOverBrightnessReal = m;
                void 0 == a.bringForwardOnHover && (a.bringForwardOnHover = x);
                e.processDescriptionWindow(d, a);
                x = this.processCoordinates(a.x, c.realWidth);
                F = this.processCoordinates(a.y, c.realHeight);
                m = a.longitudes;
                d = a.latitudes;
                p = m.length;
                if (0 < p)
                    for (x = [], z = 0; z < p; z++) x.push(c.longitudeToCoordinate(m[z]));
                p = d.length;
                if (0 < p)
                    for (F = [], z = 0; z < p; z++) F.push(c.latitudeToCoordinate(d[z]));
                if (0 < x.length) {
                    a.segments = x.length;
                    e.dx = 0;
                    e.dy = 0;
                    var A, H, y, m = 10 * (1 - Math.abs(C));
                    10 <= m && (m = NaN);
                    1 > m && (m = 1);
                    a.arcRadius = [];
                    a.distances = [];
                    if (isNaN(m)) {
                        for (m = 0; m < x.length - 1; m++) H = Math.sqrt(Math.pow(x[m + 1] - x[m], 2) + Math.pow(F[m + 1] - F[m], 2)), a.distances[m] = H;
                        m = e.line(t, x, F, l, 1, B, h, !1, !1, !0);
                        l = e.line(t, x, F, l, .001, 5, h, !1, !1, !0)
                    } else {
                        p = 1;
                        0 > C && (p = 0);
                        z = {
                            fill: "none",
                            stroke: l,
                            "stroke-opacity": 1,
                            "stroke-width": B,
                            "fill-opacity": 0
                        };
                        void 0 !== h && 0 < h && (z["stroke-dasharray"] = h);
                        h = "";
                        for (E = 0; E < x.length - 1; E++) {
                            var J = x[E],
                                L = x[E + 1],
                                M = F[E],
                                N = F[E + 1];
                            H = Math.sqrt(Math.pow(L - J, 2) + Math.pow(N - M, 2));
                            y = H / 2 * m;
                            A = 270 + 180 * Math.acos(H / 2 / y) / Math.PI;
                            isNaN(A) && (A = 270);
                            if (J < L) {
                                var O = J,
                                    J = L,
                                    L = O,
                                    O = M,
                                    M = N,
                                    N = O;
                                A = -A
                            }
                            0 < C && (A = -A);
                            h += "M" + J + "," + M + "A" + y + "," + y + ",0,0," + p + "," + L + "," + N;
                            a.arcRadius[E] = y;
                            a.distances[E] = H
                        }
                        m = t.path(h).attr(z);
                        l = t.path(h).attr({
                            "fill-opacity": 0,
                            stroke: l,
                            "stroke-width": 5,
                            "stroke-opacity": .001,
                            fill: "none"
                        })
                    }
                    e.setCN(c, m, "map-line");
                    void 0 != a.id && e.setCN(c, m, "map-line-" + a.id);
                    e.dx = .5;
                    e.dy = .5;
                    v.push(m);
                    v.push(l);
                    m.setAttr("opacity", q);
                    if ("none" != D) {
                        var G, I, K;
                        if ("end" == D || "both" == D) q = x[x.length - 1], p = F[F.length - 1], 1 < x.length ? (h = x[x.length - 2], G = F[F.length - 2]) : (h = q, G = p), G = 180 * Math.atan((p - G) / (q - h)) / Math.PI, isNaN(A) || (G += A), I = q, K = p, G = 0 > q - h ? G - 90 : G + 90;
                        "both" == D && (q = e.polygon(t, [-u / 2, 0, u / 2], [1.5 * u, 0, 1.5 * u], n, r, 1, n, r), v.push(q), q.translate(I, K, 1, !0), isNaN(G) || q.rotate(G), e.setCN(c, m, "map-line-arrow"),
                            void 0 != a.id && e.setCN(c, m, "map-line-arrow-" + a.id), a.fixedSize && f.push(q));
                        if ("start" == D || "both" == D) q = x[0], K = F[0], 1 < x.length ? (h = x[1], I = F[1]) : (h = q, I = K), G = 180 * Math.atan((K - I) / (q - h)) / Math.PI, isNaN(A) || (G -= A), I = q, G = 0 > q - h ? G - 90 : G + 90;
                        "middle" == D && (q = x[x.length - 1], p = F[F.length - 1], 1 < x.length ? (h = x[x.length - 2], G = F[F.length - 2]) : (h = q, G = p), I = h + (q - h) / 2, K = G + (p - G) / 2, G = 180 * Math.atan((p - G) / (q - h)) / Math.PI, isNaN(A) || (A = H / 2, y -= Math.sqrt(y * y - A * A), 0 > C && (y = -y), A = Math.sin(G / 180 * Math.PI), -1 == A && (A = 1), I -= A * y, K += Math.cos(G / 180 *
                            Math.PI) * y), G = 0 > q - h ? G - 90 : G + 90);
                        q = e.polygon(t, [-u / 2, 0, u / 2], [1.5 * u, 0, 1.5 * u], n, r, 1, n, r);
                        e.setCN(c, m, "map-line-arrow");
                        void 0 != a.id && e.setCN(c, m, "map-line-arrow-" + a.id);
                        v.push(q);
                        q.translate(I, K, 1, !0);
                        isNaN(G) || q.rotate(G);
                        a.fixedSize && (f.push(q), a.arrays.push({
                            arr: f,
                            el: q
                        }));
                        a.arrowSvg = q
                    }
                    a.fixedSize && m && (c = {
                        line: m,
                        thickness: B
                    }, this.linesToResize.push(c), a.arrays.push({
                        arr: this.linesToResize,
                        el: c
                    }), c = {
                        line: l,
                        thickness: 5
                    }, this.linesToResize.push(c), a.arrays.push({
                        arr: this.linesToResize,
                        el: c
                    }));
                    a.lineSvg = m;
                    a.showAsSelected && !isNaN(w) && m.setAttr("stroke", w);
                    0 < d.length ? k.push(v) : g.push(v);
                    a.hidden && v.hide()
                }
            }
        },
        processCoordinates: function(a, b) {
            var c = [],
                d;
            for (d = 0; d < a.length; d++) {
                var f = a[d],
                    e = Number(f);
                isNaN(e) && (e = Number(f.replace("%", "")) * b / 100);
                isNaN(e) || c.push(e)
            }
            return c
        },
        reset: function() {
            this.objectsToResize = [];
            this.allSvgObjects = [];
            this.allObjects = [];
            this.linesToResize = []
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.LinesSettings = e.Class({
        construct: function(a) {
            this.cname = "LinesSettings";
            this.balloonText = "[[title]]";
            this.thickness = 1;
            this.dashLength = 0;
            this.arrowSize = 10;
            this.arrowAlpha = 1;
            this.arrow = "none";
            this.color = "#990000";
            this.descriptionWindowWidth = 250;
            this.bringForwardOnHover = !0;
            e.applyTheme(this, a, this.cname)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.MapObject = e.Class({
        construct: function(a) {
            this.fixedSize = this.mouseEnabled = !0;
            this.images = [];
            this.lines = [];
            this.areas = [];
            this.remainVisible = !0;
            this.passZoomValuesToTarget = !1;
            this.objectType = this.cname;
            e.applyTheme(this, a, "MapObject");
            this.arrays = []
        },
        deleteObject: function() {
            this.remove();
            this.parentArray && e.removeFromArray(this.parentArray, this);
            if (this.arrays)
                for (var a = 0; a < this.arrays.length; a++) e.removeFromArray(this.arrays[a].arr, this.arrays[a].el);
            this.arrays = []
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.MapArea = e.Class({
        inherits: e.MapObject,
        construct: function(a) {
            this.cname = "MapArea";
            e.MapArea.base.construct.call(this, a);
            e.applyTheme(this, a, this.cname)
        },
        validate: function() {
            this.chart.areasProcessor.createArea(this)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.MapLine = e.Class({
        inherits: e.MapObject,
        construct: function(a) {
            this.cname = "MapLine";
            this.longitudes = [];
            this.latitudes = [];
            this.x = [];
            this.y = [];
            this.segments = 0;
            this.arrow = "none";
            e.MapLine.base.construct.call(this, a);
            e.applyTheme(this, a, this.cname)
        },
        validate: function() {
            this.chart.linesProcessor.createLine(this)
        },
        remove: function() {
            var a = this.displayObject;
            a && a.remove()
        },
        getCoordinates: function(a, b) {
            isNaN(b) && (b = 0);
            if (!isNaN(a)) {
                var c, d, f, k, g, h;
                1 < this.longitudes.length ? (c = this.chart.longitudeToCoordinate(this.longitudes[b]), f = this.chart.longitudeToCoordinate(this.longitudes[b + 1]), d = this.chart.latitudeToCoordinate(this.latitudes[b]), k = this.chart.latitudeToCoordinate(this.latitudes[b + 1])) : 1 < this.x.length && (c = this.x[b], f = this.x[b + 1], d = this.y[b], k = this.y[b + 1]);
                var l = Math.sqrt(Math.pow(f - c, 2) + Math.pow(k - d, 2));
                c < f && !isNaN(this.arc) && (a = 1 - a);
                g = c + (f - c) * a;
                h = d + (k - d) * a;
                var n = Math.atan2(k - d, f - c);
                if (!isNaN(this.arc) && this.arcRadius) {
                    var m = 0;
                    c < f && (m = c, c = f, f = m, m = d, d = k, k = m, m = Math.PI);
                    h = this.arcRadius[b];
                    0 > this.arc && (l = -l);
                    g = c + (f - c) / 2 + Math.sqrt(h * h - l / 2 * (l / 2)) * (d - k) / l;
                    var r = d + (k - d) / 2 + Math.sqrt(h * h - l / 2 * (l / 2)) * (f - c) / l;
                    c = 180 * Math.atan2(d - r, c - g) / Math.PI;
                    n = e.degreesToRadians(c + (180 * Math.atan2(k - r, f - g) / Math.PI - c) * a);
                    g += h * Math.cos(n);
                    h = r + h * Math.sin(n);
                    n = 0 < this.arc ? n + Math.PI / 2 : n - Math.PI / 2;
                    n += m
                }
                this.distance = l;
                return {
                    x: g,
                    y: h,
                    angle: n
                }
            }
        },
        fixToStage: function() {
            if (0 < this.latitudes.length) {
                this.y = [];
                for (var a = 0; a < this.latitudes.length; a++) this.y.push(this.chart.latitudeToStageY(this.latitudes[a]));
                this.latitudes = [];
                this.x = [];
                for (a = 0; a < this.longitudes.length; a++) this.x.push(this.chart.longitudeToStageX(this.longitudes[a]));
                this.longitudes = []
            }
            this.validate()
        },
        fixToMap: function() {
            if (0 < this.y.length) {
                this.latitudes = [];
                for (var a = 0; a < this.y.length; a++) this.latitudes.push(this.chart.stageYToLatitude(this.y[a]));
                this.y = [];
                this.longitudes = [];
                for (a = 0; a < this.x.length; a++) this.longitudes.push(this.chart.stageXToLongitude(this.x[a]));
                this.x = []
            }
            this.validate()
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.MapImage = e.Class({
        inherits: e.MapObject,
        construct: function(a) {
            this.cname = "MapImage";
            this.scale = 1;
            this.widthAndHeightUnits = "pixels";
            this.labelShiftY = this.labelShiftX = 0;
            this.positionOnLine = .5;
            this.direction = 1;
            this.lineSegment = this.extraAngle = 0;
            this.createEvents("animationStart", "animationEnd");
            e.MapImage.base.construct.call(this, a);
            e.applyTheme(this, a, this.cname)
        },
        validate: function() {
            this.chart.imagesProcessor.createImage(this)
        },
        updatePosition: function() {
            this.chart.imagesProcessor.updateSizeAndPosition(this)
        },
        remove: function() {
            var a = this.displayObject;
            a && a.remove();
            (a = this.imageLabel) && a.remove()
        },
        animateTo: function(a, b, c, d) {
            isNaN(c) || (this.animationDuration = c);
            d && (this.easingFunction = d);
            this.finalX = a;
            this.finalY = b;
            isNaN(this.longitude) || (this.initialX = this.longitude);
            isNaN(this.left) || (this.initialX = this.left);
            isNaN(this.right) || (this.initialX = this.right);
            isNaN(this.latitude) || (this.initialY = this.latitude);
            isNaN(this.top) || (this.initialY = this.top);
            isNaN(this.bottom) || (this.initialY = this.bottom);
            this.animatingAlong = !1;
            this.animate()
        },
        animateAlong: function(a, b, c) {
            isNaN(b) || (this.animationDuration = b);
            c && (this.easingFunction = c);
            a && (this.line = this.chart.getObjectById(a));
            this.animatingAlong = !0;
            this.animate()
        },
        animate: function() {
            var a = this,
                b = a.chart.imagesSettings,
                c = a.animationDuration;
            isNaN(c) && (c = b.animationDuration);
            a.totalFrames = Math.round(1E3 * c / e.updateRate);
            c = 1;
            a.line && b.adjustAnimationSpeed && (a.line.distances && (c = a.line.distances[a.lineSegment] * a.chart.zoomLevel(), c = Math.abs(c / b.baseAnimationDistance)), a.totalFrames = Math.round(c * a.totalFrames));
            a.frame = 0;
            a.clearTO();
            a.timeOut = setTimeout(function() {
                a.update.call(a)
            }, e.updateRate);
            b = {
                type: "animationStart",
                chart: a.chart,
                image: this,
                lineSegment: a.lineSegment,
                direction: a.direction
            };
            a.fire(b.type, b)
        },
        clearTO: function() {
            this.timeOut && clearTimeout(this.timeOut)
        },
        update: function() {
            var a = this;
            a.updatePosition();
            var b = Math.round(1E3 / e.updateRate),
                c = a.chart.imagesSettings,
                d = a.easingFunction;
            d || (d = c.easingFunction);
            a.frame++;
            c = a.totalFrames;
            a.frame <= c ? (d = d(0, a.frame, 0, 1, c), -1 == a.direction && (d = 1 - d), a.animatingAlong ? a.positionOnLine = d : (c = a.initialX + (a.finalX - a.initialX) * d, isNaN(a.longitude) || (a.longitude = c), isNaN(a.left) || (a.left = c), isNaN(a.right) || (a.right = c), d = a.initialY + (a.finalY - a.initialY) * d, isNaN(a.latitude) || (a.latitude = d), isNaN(a.top) || (a.top = d), isNaN(a.bottom) || (a.bottom = d)), a.clearTO(), a.timeOut = setTimeout(function() {
                a.update.call(a)
            }, b)) : (b = {
                type: "animationEnd",
                chart: a.chart,
                image: this,
                lineSegment: a.lineSegment,
                direction: a.direction
            }, a.fire(b.type, b), a.animatingAlong && (1 == a.direction ? a.lineSegment < a.line.segments - 2 ? (a.lineSegment++, a.delayAnimateAlong(), a.positionOnLine = 0) : a.flipDirection ? (a.direction = -1, a.extraAngle = 180, a.delayAnimateAlong()) : a.loop && (a.delayAnimateAlong(), a.lineSegment = 0) : 0 < a.lineSegment ? (a.lineSegment--, a.delayAnimateAlong(), a.positionOnLine = 0) : a.loop && a.flipDirection ? (a.direction = 1, a.extraAngle = 0, a.delayAnimateAlong()) : a.loop && a.delayAnimateAlong()))
        },
        delayAnimateAlong: function() {
            var a = this;
            a.clearTO();
            a.animateAlongLine && (a.timeOut = setTimeout(function() {
                a.animateAlong.call(a)
            }, 1E3 * a.chart.imagesSettings.pauseDuration))
        },
        fixToStage: function() {
            isNaN(this.longitude) || (this.left = this.chart.longitudeToStageX(this.longitude), this.longitude = void 0);
            isNaN(this.latitude) || (this.top = this.chart.latitudeToStageY(this.latitude), this.latitude = void 0);
            this.validate()
        },
        fixToMap: function() {
            isNaN(this.left) || (this.longitude = this.chart.stageXToLongitude(this.left), this.left = void 0);
            isNaN(this.top) || (this.latitude = this.chart.stageYToLatitude(this.top), this.top = void 0);
            this.validate()
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.degreesToRadians = function(a) {
        return a / 180 * Math.PI
    };
    e.radiansToDegrees = function(a) {
        return a / Math.PI * 180
    };
    e.getColorFade = function(a, b, c) {
        var d = e.hex2RGB(b);
        b = d[0];
        var f = d[1],
            d = d[2],
            k = e.hex2RGB(a);
        a = k[0];
        var g = k[1],
            k = k[2];
        a += Math.round((b - a) * c);
        g += Math.round((f - g) * c);
        k += Math.round((d - k) * c);
        return "rgb(" + a + "," + g + "," + k + ")"
    };
    e.hex2RGB = function(a) {
        return [parseInt(a.substring(1, 3), 16), parseInt(a.substring(3, 5), 16), parseInt(a.substring(5, 7), 16)]
    };
    e.processDescriptionWindow = function(a, b) {
        isNaN(b.descriptionWindowX) && (b.descriptionWindowX = a.descriptionWindowX);
        isNaN(b.descriptionWindowY) && (b.descriptionWindowY = a.descriptionWindowY);
        isNaN(b.descriptionWindowLeft) && (b.descriptionWindowLeft = a.descriptionWindowLeft);
        isNaN(b.descriptionWindowRight) && (b.descriptionWindowRight = a.descriptionWindowRight);
        isNaN(b.descriptionWindowTop) && (b.descriptionWindowTop = a.descriptionWindowTop);
        isNaN(b.descriptionWindowBottom) && (b.descriptionWindowBottom = a.descriptionWindowBottom);
        isNaN(b.descriptionWindowWidth) && (b.descriptionWindowWidth = a.descriptionWindowWidth);
        isNaN(b.descriptionWindowHeight) && (b.descriptionWindowHeight = a.descriptionWindowHeight)
    }
})();
(function() {
    var e = window.AmCharts;
    e.MapData = e.Class({
        inherits: e.MapObject,
        construct: function() {
            this.cname = "MapData";
            e.MapData.base.construct.call(this);
            this.projection = "mercator";
            this.topLatitude = 90;
            this.bottomLatitude = -90;
            this.leftLongitude = -180;
            this.rightLongitude = 180;
            this.zoomLevel = 1;
            this.getAreasFromMap = !1
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.DescriptionWindow = e.Class({
        construct: function() {},
        show: function(a, b, c, d) {
            var f = this,
                e = document.createElement("div");
            e.style.position = "absolute";
            var g = a.classNamePrefix + "-description-";
            e.className = "ammapDescriptionWindow " + g + "div";
            f.div = e;
            b.appendChild(e);
            var h = ".gif";
            a.svgIcons && (h = ".svg");
            var l = document.createElement("img");
            l.className = "ammapDescriptionWindowCloseButton " + g + "close-img";
            l.src = a.pathToImages + "xIcon" + h;
            l.style.cssFloat = "right";
            l.style.cursor = "pointer";
            l.onclick = function() {
                f.close()
            };
            l.onmouseover = function() {
                l.src = a.pathToImages + "xIconH" + h
            };
            l.onmouseout = function() {
                l.src = a.pathToImages + "xIcon" + h
            };
            e.appendChild(l);
            b = document.createElement("div");
            b.className = "ammapDescriptionTitle " + g + "title-div";
            b.onmousedown = function() {
                f.div.style.zIndex = 1E3
            };
            e.appendChild(b);
            d = document.createTextNode(d);
            b.appendChild(d);
            d = b.offsetHeight;
            b = document.createElement("div");
            b.className = "ammapDescriptionText " + g + "text-div";
            b.style.maxHeight = f.maxHeight - d - 20 + "px";
            e.appendChild(b);
            b.innerHTML = c
        },
        close: function() {
            try {
                this.div.parentNode.removeChild(this.div)
            } catch (a) {}
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.ValueLegend = e.Class({
        construct: function(a) {
            this.cname = "ValueLegend";
            this.enabled = !0;
            this.showAsGradient = !1;
            this.minValue = 0;
            this.height = 12;
            this.width = 200;
            this.bottom = this.left = 10;
            this.borderColor = "#FFFFFF";
            this.borderAlpha = this.borderThickness = 1;
            this.color = "#000000";
            this.fontSize = 11;
            e.applyTheme(this, a, this.cname)
        },
        init: function(a, b) {
            if (this.enabled) {
                var c = a.areasSettings.color,
                    d = a.areasSettings.colorSolid,
                    f = a.colorSteps;
                e.remove(this.set);
                var k = b.set();
                this.set = k;
                e.setCN(a, k, "value-legend");
                var g = 0,
                    h = this.minValue,
                    l = this.fontSize,
                    n = a.fontFamily,
                    m = this.color;
                void 0 == h && (h = a.minValueReal);
                void 0 !== h && (g = e.text(b, h, m, n, l, "left"), g.translate(0, l / 2 - 1), e.setCN(a, g, "value-legend-min-label"), k.push(g), g = g.getBBox().height);
                h = this.maxValue;
                void 0 === h && (h = a.maxValueReal);
                void 0 !== h && (g = e.text(b, h, m, n, l, "right"), g.translate(this.width, l / 2 - 1), e.setCN(a, g, "value-legend-max-label"), k.push(g), g = g.getBBox().height);
                if (this.showAsGradient) c = e.rect(b, this.width, this.height, [c, d], 1, this.borderThickness, this.borderColor, 1, 0, 0), e.setCN(a, c, "value-legend-gradient"), c.translate(0, g), k.push(c);
                else
                    for (l = this.width / f, n = 0; n < f; n++) m = e.getColorFade(c, d, 1 * n / (f - 1)), m = e.rect(b, l, this.height, m, 1, this.borderThickness, this.borderColor, 1), e.setCN(a, m, "value-legend-color"), e.setCN(a, m, "value-legend-color-" + n), m.translate(l * n, g), k.push(m);
                d = c = 0;
                f = k.getBBox();
                g = a.getY(this.bottom, !0);
                l = a.getY(this.top);
                n = a.getX(this.right, !0);
                m = a.getX(this.left);
                isNaN(l) || (c = l);
                isNaN(g) || (c = g - f.height);
                isNaN(m) || (d = m);
                isNaN(n) || (d = n - f.width);
                k.translate(d, c)
            } else e.remove(this.set)
        }
    })
})();
(function() {
    var e = window.AmCharts;
    e.ObjectList = e.Class({
        construct: function(a) {
            this.divId = a
        },
        init: function(a) {
            this.chart = a;
            var b = this.divId;
            this.container && (b = this.container);
            this.div = "object" != typeof b ? document.getElementById(b) : b;
            b = document.createElement("div");
            b.className = "ammapObjectList " + a.classNamePrefix + "-object-list-div";
            this.div.appendChild(b);
            this.addObjects(a.dataProvider, b)
        },
        addObjects: function(a, b) {
            var c = this.chart,
                d = document.createElement("ul");
            d.className = c.classNamePrefix + "-object-list-ul";
            var e;
            if (a.areas)
                for (e = 0; e < a.areas.length; e++) {
                    var k = a.areas[e];
                    void 0 === k.showInList && (k.showInList = c.showAreasInList);
                    this.addObject(k, d)
                }
            if (a.images)
                for (e = 0; e < a.images.length; e++) k = a.images[e], void 0 === k.showInList && (k.showInList = c.showImagesInList), this.addObject(k, d);
            if (a.lines)
                for (e = 0; e < a.lines.length; e++) k = a.lines[e], void 0 === k.showInList && (k.showInList = c.showLinesInList), this.addObject(k, d);
            0 < d.childNodes.length && b.appendChild(d)
        },
        addObject: function(a, b) {
            var c = this;
            if (a.showInList && void 0 !== a.title) {
                var d = c.chart,
                    e = document.createElement("li");
                e.className = d.classNamePrefix + "-object-list-li";
                var k = a.titleTr;
                k || (k = a.title);
                var k = document.createTextNode(k),
                    g = document.createElement("a");
                g.className = d.classNamePrefix + "-object-list-a";
                g.appendChild(k);
                e.appendChild(g);
                b.appendChild(e);
                this.addObjects(a, e);
                g.onmouseover = function() {
                    c.chart.rollOverMapObject(a, !1)
                };
                g.onmouseout = function() {
                    c.chart.rollOutMapObject(a)
                };
                g.onclick = function() {
                    c.chart.clickMapObject(a)
                }
            }
        }
    })
})();