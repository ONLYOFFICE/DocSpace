import * as React from "react";
import ViewerImage from "./viewer-image";
import classnames from "classnames";
import ViewerToolbar, { defaultToolbars } from "./viewer-toolbar";
import { isMobileOnly } from "react-device-detect";
import Icon, { ActionType } from "./icon";

const ACTION_TYPES = {
  setVisible: "setVisible",
  setActiveIndex: "setActiveIndex",
  update: "update",
  clear: "clear",
};

function createAction(type, payload) {
  return {
    type,
    payload: payload || {},
  };
}

const ViewerBase = (props) => {
  const {
    visible = false,
    images = [],
    activeIndex = 0,
    zIndex = 300,
    drag = true,
    attribute = true,
    zoomable = true,
    rotatable = true,
    scalable = true,
    changeable = true,
    customToolbar = (toolbars) => toolbars,
    zoomSpeed = 0.25,
    disableKeyboardSupport = false,
    noResetZoomAfterChange = false,
    noLimitInitializationSize = false,
    defaultScale = 1,
    loop = true,
    disableMouseZoom = false,
    downloadable = false,
    noImgDetails = false,
    noToolbar = false,
    showTotal = true,
    totalName = "of",
    minScale = 0.1,
    generateContextMenu,
    mobileDetails,
    onNextClick,
    onPrevClick,
    onMaskClick,
    isPreviewFile,
    archiveRoom,
  } = props;

  const initialState = {
    visible: false,
    visibleStart: false,
    transitionEnd: false,
    activeIndex: props.activeIndex,
    width: 0,
    height: 0,
    top: 15,
    left: null,
    rotate: 0,
    imageWidth: 0,
    imageHeight: 0,
    scaleX: defaultScale,
    scaleY: defaultScale,
    loading: false,
    loadFailed: false,
    startLoading: false,
    percent: 100,
    withTransition: false,
    opacity: 1,
  };
  function setContainerWidthHeight() {
    let width = window.innerWidth;
    let height = window.innerHeight;
    return {
      width,
      height,
    };
  }
  const containerSize = React.useRef(setContainerWidthHeight());
  const imageRef = React.useRef(null);

  const [isOpen, setIsOpen] = React.useState(false);
  const footerHeight = 0;
  function reducer(state, action) {
    switch (action.type) {
      case ACTION_TYPES.setVisible:
        return {
          ...state,
          visible: action.payload.visible,
        };
      case ACTION_TYPES.setActiveIndex:
        return {
          ...state,
          activeIndex: action.payload.index,
          startLoading: true,
        };
      case ACTION_TYPES.update:
        return {
          ...state,
          ...action.payload,
        };
      case ACTION_TYPES.clear:
        return {
          ...state,
          width: 0,
          height: 0,
          scaleX: defaultScale,
          scaleY: defaultScale,
          rotate: 1,
          imageWidth: 0,
          imageHeight: 0,
          loadFailed: false,
          top: 0,
          left: 0,
          loading: false,
        };
      default:
        break;
    }
    return state;
  }

  const viewerCore = React.useRef(null);
  const init = React.useRef(false);
  const currentLoadIndex = React.useRef(0);

  const [state, dispatch] = React.useReducer(reducer, initialState);

  React.useEffect(() => {
    init.current = true;

    return () => {
      init.current = false;
    };
  }, []);

  React.useEffect(() => {
    containerSize.current = setContainerWidthHeight();
  }, [props.container]);

  React.useEffect(() => {
    if (visible) {
      if (init.current) {
        dispatch(
          createAction(ACTION_TYPES.setVisible, {
            visible: true,
          })
        );
      }
    }
  }, [visible]);

  React.useEffect(() => {
    bindEvent();

    return () => {
      bindEvent(true);
    };
  });

  React.useEffect(() => {
    //fix memory leak
    if (!init.current) return;

    if (visible) {
      if (!props.container) {
        document.body.style.overflow = "hidden";
        if (document.body.scrollHeight > document.body.clientHeight) {
          document.body.style.paddingRight = "15px";
        }
      }
    } else {
      dispatch(createAction(ACTION_TYPES.clear, {}));
    }

    return () => {
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
    };
  }, [state.visible]);

  React.useEffect(() => {
    //fix memory leak
    if (!init.current) return;

    if (visible) {
      dispatch(
        createAction(ACTION_TYPES.setActiveIndex, {
          index: activeIndex,
        })
      );
    }
  }, [activeIndex, visible, images]);

  function loadImg(currentActiveIndex, isReset = false) {
    const timerId = setTimeout(() => {
      dispatch(
        createAction(ACTION_TYPES.update, {
          loading: true,
        })
      );
    }, 300);
    dispatch(
      createAction(ACTION_TYPES.update, {
        loadFailed: false,
      })
    );
    let activeImage = null;
    if (images.length > 0) {
      activeImage = images[currentActiveIndex];
    }

    if (imageRef.current) {
      //abort previous image request
      imageRef.current.src = ""
    }


    let loadComplete = false;
    let img = new Image();
    imageRef.current = img
    img.src = activeImage.src;

    img.onload = () => {
      clearTimeout(timerId);
      if (!init.current) {
        return;
      }
      if (!loadComplete) {
        loadImgSuccess(img.width, img.height, true);
      }
    };
    img.onerror = () => {
      clearTimeout(timerId);
      dispatch(
        createAction(ACTION_TYPES.update, {
          loading: false,
          loadFailed: false,
          startLoading: false,
        })
      );
    };
    if (img.complete) {
      loadComplete = true;
      loadImgSuccess(img.width, img.height, true);
    }
    function loadImgSuccess(imgWidth, imgHeight, success) {
      if (currentActiveIndex !== currentLoadIndex.current) {
        return;
      }
      let realImgWidth = imgWidth;
      let realImgHeight = imgHeight;

      let [width, height] = getImgWidthHeight(realImgWidth, realImgHeight);
      let left = (containerSize.current.width - width) / 2;
      let top = (containerSize.current.height - height - footerHeight) / 2;

      let scaleX = defaultScale;
      let scaleY = defaultScale;
      if (noResetZoomAfterChange && !isReset) {
        scaleX = state.scaleX;
        scaleY = state.scaleY;
      }
      props.setPanelVisible(true);
      dispatch(
        createAction(ACTION_TYPES.update, {
          width: width,
          height: height,
          left: left,
          top: top,
          imageWidth: imgWidth,
          imageHeight: imgHeight,
          loading: false,
          rotate: 0,
          scaleX: scaleX,
          scaleY: scaleY,
          loadFailed: !success,
          startLoading: false,
          percent: 100,
          opacity: 1,
        })
      );
    }
  }

  React.useEffect(() => {
    if (state.startLoading) {
      currentLoadIndex.current = state.activeIndex;
      loadImg(state.activeIndex);
    }
  }, [state.startLoading, state.activeIndex, images[state.activeIndex]?.src]);

  function getImgWidthHeight(imgWidth, imgHeight) {
    const titleHeight = 0;

    let width = 0;
    let height = 0;
    let maxWidth = containerSize.current.width;

    let maxHeight = containerSize.current.height - (footerHeight + titleHeight);

    width = Math.min(maxWidth, imgWidth);
    height = (width / imgWidth) * imgHeight;

    if (height > maxHeight) {
      height = maxHeight;
      width = (height / imgHeight) * imgWidth;
    }
    if (noLimitInitializationSize) {
      width = imgWidth;
      height = imgHeight;
    }
    return [width, height];
  }

  function onPercentClick() {
    if (state.percent === 100) return;
    let imgCenterXY = getImageCenterXY();

    const zoomCondition = state.percent < 100;

    const direct = zoomCondition ? 1 : -1;
    const zoom = zoomCondition
      ? (100 - state.percent) / 100
      : (state.percent - 100) / 100;

    handleZoom(imgCenterXY.x, imgCenterXY.y, direct, zoom);
  }

  function getActiveImage(activeIndex2 = undefined) {
    let activeImg2 = {
      src: "",
      alt: "",
      downloadUrl: "",
    };

    let realActiveIndex = null;
    if (activeIndex2 !== undefined) {
      realActiveIndex = activeIndex2;
    } else {
      realActiveIndex = state.activeIndex;
    }
    if (images.length > 0 && realActiveIndex >= 0) {
      activeImg2 = images[realActiveIndex];
    }

    return activeImg2;
  }

  function handleDownload() {
    const activeImage = getActiveImage();
    if (activeImage.downloadUrl) {
      if (props.downloadInNewWindow) {
        window.open(activeImage.downloadUrl, "_blank");
      } else {
        location.href = activeImage.downloadUrl;
      }
    }
  }

  function handleRotate(isRight = false) {
    dispatch(
      createAction(ACTION_TYPES.update, {
        withTransition: true,
        rotate: state.rotate + 90 * (isRight ? 1 : -1),
      })
    );
  }

  function handleDefaultAction(type) {
    switch (type) {
      case ActionType.zoomIn:
        let imgCenterXY = getImageCenterXY();
        handleZoom(imgCenterXY.x, imgCenterXY.y, 1, zoomSpeed);
        break;
      case ActionType.zoomOut:
        let imgCenterXY2 = getImageCenterXY();
        handleZoom(imgCenterXY2.x, imgCenterXY2.y, -1, zoomSpeed);
        break;
      case ActionType.rotateLeft:
        handleRotate();
        break;
      case ActionType.rotateRight:
        handleRotate(true);
        break;
      case ActionType.reset:
        loadImg(state.activeIndex, true);
        break;
      case ActionType.download:
        handleDownload();
        break;
      default:
        break;
    }
  }

  function handleAction(config) {
    handleDefaultAction(config.actionType);

    if (config.onClick) {
      const activeImage = getActiveImage();
      config.onClick(activeImage);
    }
  }

  function handleChangeImgState(width, height, top, left) {
    dispatch(
      createAction(ACTION_TYPES.update, {
        width: width,
        height: height,
        top: top,
        left: left,
        withTransition: false,
      })
    );
  }

  function handleResize() {
    containerSize.current = setContainerWidthHeight();
    if (visible) {
      const [imgWidth, imgHeight] = getImgWidthHeight(
        state.imageWidth,
        state.imageHeight
      );

      let left = (containerSize.current.width - imgWidth) / 2;
      let top =
        (containerSize.current.height - imgHeight - (footerHeight - 53)) / 2;

      dispatch(
        createAction(ACTION_TYPES.update, {
          left: left,
          top: top,
          width: imgWidth,
          height: imgHeight,
        })
      );
      viewerCore;
    }
  }

  function bindEvent(remove = false) {
    let funcName = "addEventListener";
    if (remove) {
      funcName = "removeEventListener";
    }
    if (!disableKeyboardSupport) {
      document[funcName]("keydown", handleKeydown, true);
    }
    if (viewerCore.current) {
      viewerCore.current[funcName]("wheel", handleMouseScroll, false);
    }
  }

  function handleKeydown(e) {
    let keyCode = e.keyCode || e.which || e.charCode;
    let isFeatrue = false;
    switch (keyCode) {
      // key: ↑
      case 38:
        handleDefaultAction(ActionType.zoomIn);
        isFeatrue = true;
        break;
      // key: ↓
      case 40:
        handleDefaultAction(ActionType.zoomOut);
        isFeatrue = true;
        break;
      // key: Ctrl + 1
      case 49:
        if (e.ctrlKey) {
          loadImg(state.activeIndex);
          isFeatrue = true;
        }
        break;
      default:
        break;
    }
    if (isFeatrue) {
      e.preventDefault();
      e.stopPropagation();
    }
  }

  function handleMouseScroll(e) {
    if (disableMouseZoom) {
      return;
    }
    if (state.loading) {
      return;
    }
    e.preventDefault();
    let direct = 0;
    const value = e.deltaY;
    if (value === 0) {
      direct = 0;
    } else {
      direct = value > 0 ? -1 : 1;
    }
    if (direct !== 0) {
      let imgCenterXY = getImageCenterXY();
      handleZoom(imgCenterXY.x, imgCenterXY.y, direct, zoomSpeed);
    }
  }

  function getImageCenterXY() {
    return {
      x: state.left + state.width / 2,
      y: state.top + state.height / 2,
    };
  }

  function handleResetZoom() {
    const [imgWidth, imgHeight] = getImgWidthHeight(
      state.imageWidth,
      state.imageHeight
    );
    const left = (containerSize.current.width - imgWidth) / 2;
    const top = (containerSize.current.height - imgHeight) / 2;
    dispatch(
      createAction(ACTION_TYPES.update, {
        width: imgWidth,
        height: imgHeight,
        scaleX: 1,
        scaleY: 1,
        top: top,
        left: left,
        loading: false,
        percent: 100,
        withTransition: true,
      })
    );
  }

  function handleZoom(targetX, targetY, direct, scale) {
    let imgCenterXY = getImageCenterXY();
    let diffX = targetX - imgCenterXY.x;
    let diffY = targetY - imgCenterXY.y;
    let top = 0;
    let left = 0;
    let width = 0;
    let height = 0;
    let scaleX = 0;
    let scaleY = 0;
    let zoomPercent =
      direct === 1 ? state.percent + scale * 100 : state.percent - scale * 100;
    if (zoomPercent === 0) return;
    if (scale === 1) {
      zoomPercent = 100;
    }

    let nowWidth = state.width;

    if (nowWidth === 0) {
      const [imgWidth, imgHeight] = getImgWidthHeight(
        state.imageWidth,
        state.imageHeight
      );
      left = (containerSize.current.width - imgWidth) / 2;
      top = (containerSize.current.height - footerHeight - imgHeight) / 2;
      width = state.width + imgWidth;
      height = state.height + imgHeight;
      scaleX = scaleY = 1;
    } else {
      let directX = state.scaleX > 0 ? 1 : -1;
      let directY = state.scaleY > 0 ? 1 : -1;
      scaleX = state.scaleX + scale * direct * directX;
      scaleY = state.scaleY + scale * direct * directY;
      if (typeof props.maxScale !== "undefined") {
        if (Math.abs(scaleX) > props.maxScale) {
          scaleX = props.maxScale * directX;
        }
        if (Math.abs(scaleY) > props.maxScale) {
          scaleY = props.maxScale * directY;
        }
      }
      if (Math.abs(scaleX) < minScale) {
        scaleX = minScale * directX;
      }
      if (Math.abs(scaleY) < minScale) {
        scaleY = minScale * directY;
      }
      top = state.top + ((-direct * diffY) / state.scaleX) * scale * directX;
      left = state.left + ((-direct * diffX) / state.scaleY) * scale * directY;
      width = nowWidth;
      height = state.height;
    }
    dispatch(
      createAction(ACTION_TYPES.update, {
        width: width,
        scaleX: scaleX > 0 ? scaleX : 0,
        scaleY: scaleY > 0 ? scaleY : 0,
        height: height,
        top: top,
        left: left,
        loading: false,
        percent: zoomPercent,
        withTransition: true,
      })
    );

    return [scaleX, scaleY];
  }

  let currentTop = (containerSize.current.height - state.height) / 2;
  const prefixCls = "react-viewer";

  const className = classnames(`${prefixCls}`, `${prefixCls}-transition`, {
    [`${prefixCls}-inline`]: props.container,
    [props.className]: props.className,
  });

  let viewerStyle = {
    opacity: visible && state.visible ? 1 : 0,
    display: visible || state.visible ? "block" : "none",
  };

  let activeImg = {
    src: "",
    alt: "",
  };

  if (
    visible &&
    state.visible &&
    !state.loading &&
    state.activeIndex !== null &&
    !state.startLoading
  ) {
    activeImg = getActiveImage();
  }

  const displayVisible = JSON.parse(localStorage.getItem("displayVisible"));
  return (
    <div
      className={className}
      style={viewerStyle}
      id="image-viewer"
      onTransitionEnd={() => {
        if (!visible) {
          dispatch(
            createAction(ACTION_TYPES.setVisible, {
              visible: false,
            })
          );
        }
      }}
      ref={viewerCore}
    >
      {isMobileOnly && !displayVisible && mobileDetails}
      <div
        className={`${prefixCls}-mask`}
        style={{
          zIndex: zIndex,
          backgroundColor: `${isMobileOnly
              ? !displayVisible
                ? "rgba(55,55,55,0.6)"
                : "#000"
              : "rgba(55,55,55,0.6)"
            }`,
        }}
      />
      <ViewerImage
        prefixCls={prefixCls}
        imgSrc={
          state.loadFailed
            ? props.defaultImg.src || activeImg.src
            : activeImg.src
        }
        visible={visible}
        width={state.width}
        dispatch={dispatch}
        createAction={createAction}
        actionType={ACTION_TYPES}
        playlist={props.playlist}
        playlistPos={props.playlistPos}
        currentTop={currentTop}
        opacity={state.opacity}
        getImageCenterXY={getImageCenterXY}
        setPanelVisible={props.setPanelVisible}
        handleZoom={handleZoom}
        handleResetZoom={handleResetZoom}
        height={state.height}
        onNextClick={onNextClick}
        onPrevClick={onPrevClick}
        tpCache={state.tpCache}
        withTransition={state.withTransition}
        top={state.top}
        left={state.left}
        rotate={state.rotate}
        onMaskClick={onMaskClick}
        needUpdatePoint={state.needUpdatePoint}
        onChangeImgState={handleChangeImgState}
        onResize={handleResize}
        zIndex={zIndex + 5}
        scaleX={state.scaleX}
        containerSize={containerSize}
        scaleY={state.scaleY}
        loading={state.loading}
        drag={drag}
        container={props.container}
      />
      {props.noFooter ||
        (!isMobileOnly && props.displayUI && (
          <div className={`${prefixCls}-container`}>
            <div
              className={`${prefixCls}-footer`}
              style={{ zIndex: zIndex + 7 }}
            >
              {noToolbar || (
                <ViewerToolbar
                  isMobileOnly={isMobileOnly}
                  imageTimer={props.imageTimer}
                  prefixCls={prefixCls}
                  onAction={handleAction}
                  alt={activeImg.alt}
                  width={state.imageWidth}
                  height={state.imageHeight}
                  percent={state.percent}
                  attribute={attribute}
                  isPreviewFile={isPreviewFile}
                  archiveRoom={archiveRoom}
                  setIsOpenContextMenu={props.setIsOpenContextMenu}
                  zoomable={zoomable}
                  rotatable={rotatable}
                  onPercentClick={onPercentClick}
                  generateContextMenu={generateContextMenu}
                  scalable={scalable}
                  changeable={changeable}
                  downloadable={downloadable}
                  isOpen={isOpen}
                  setIsOpen={setIsOpen}
                  noImgDetails={noImgDetails}
                  toolbars={customToolbar(defaultToolbars)}
                  activeIndex={state.activeIndex}
                  count={images.length}
                  showTotal={showTotal}
                  totalName={totalName}
                />
              )}
            </div>
          </div>
        ))}
    </div>
  );
};

export { ViewerBase };
