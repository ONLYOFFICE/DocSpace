import React from "react";

import Dropzone from "react-dropzone";
import ReactAvatarEditor from "./react-avatar-editor";
import PropTypes from "prop-types";
import Avatar from "../../avatar/index";
import accepts from "attr-accept";
import Text from "../../text";
import Box from "../../box";
import ContextMenuButton from "../../context-menu-button";
import IconButton from "../../icon-button";

import Slider from "../../slider";
import {
  isDesktop,
  //isTablet,
  //isMobile
} from "../../utils/device";
import resizeImage from "resize-image";
import throttle from "lodash/throttle";
import Link from "../../link";

const step = 0.01;
const min = 1;
const max = 5;

import {
  StyledAvatarEditorBody,
  StyledAvatarContainer,
  DropZoneContainer,
  StyledErrorContainer,
} from "./styled-avatar-editor-body";

class AvatarEditorBody extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      image: this.props.image ? this.props.image : "",
      scale: 1,
      croppedImage: "",
      errorText: null,
      rotate: 0,
    };

    this.setEditorRef = React.createRef();
    this.dropzoneRef = React.createRef();

    this.throttledSetCroppedImage = throttle(this.setCroppedImage, 300);
  }

  onPositionChange = (position) => {
    this.props.onPositionChange({
      x: position.x,
      y: position.y,
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height,
    });
  };

  onDropRejected = (rejectedFiles) => {
    if (!accepts(rejectedFiles[0], this.props.accept)) {
      this.props.onLoadFileError(0);
      this.setState({
        errorText: this.props.unknownTypeError,
      });
      return;
    } else if (rejectedFiles[0].size > this.props.maxSize) {
      this.props.onLoadFileError(1);
      this.setState({
        errorText: this.props.maxSizeFileError,
      });
      return;
    }
    this.setState({
      errorText: this.props.unknownError,
    });
    this.props.onLoadFileError(2);
  };

  onDropAccepted = (acceptedFiles) => {
    const _this = this;
    var fr = new FileReader();
    fr.readAsDataURL(acceptedFiles[0]);
    fr.onload = function () {
      var img = new Image();
      img.onload = function () {
        var canvas = resizeImage.resize2Canvas(img, img.width, img.height);
        var data = resizeImage.resize(
          canvas,
          img.width / 4,
          img.height / 4,
          resizeImage.JPEG
        );
        _this.setState({
          image: data,
          rotate: 0,
          errorText: null,
        });
        fetch(data)
          .then((res) => res.blob())
          .then((blob) => {
            const file = new File([blob], "File name", {
              type: "image/jpg",
            });
            //console.log(`file size ${file.size / 1024 / 1024} mb`);
            _this.props.onLoadFile(file);
          });
      };
      img.src = fr.result;
    };
  };

  deleteImage = () => {
    this.setState({
      image: "",
      rotate: 0,
      croppedImage: "",
    });
    this.props.deleteImage();
  };

  setCroppedImage = () => {
    if (this.setEditorRef && this.setEditorRef.current) {
      const image = this.setEditorRef.current.getImage()?.toDataURL();
      this.setState({
        croppedImage: image,
      });
      this.props.onImageChange(image);
    }
  };

  dist = 0;
  scaling = false;
  curr_scale = 1.0;
  scale_factor = 1.0;

  distance = (p1, p2) => {
    return Math.sqrt(
      Math.pow(p1.clientX - p2.clientX, 2) +
        Math.pow(p1.clientY - p2.clientY, 2)
    );
  };

  onTouchStart = (evt) => {
    //evt.preventDefault();
    var tt = evt.targetTouches;
    if (tt.length >= 2) {
      this.dist = this.distance(tt[0], tt[1]);
      this.scaling = true;
    } else {
      this.scaling = false;
    }
  };

  onTouchMove = (evt) => {
    //evt.preventDefault();
    var tt = evt.targetTouches;
    if (this.scaling) {
      this.curr_scale =
        (this.distance(tt[0], tt[1]) / this.dist) * this.scale_factor;

      this.setState({
        scale:
          this.curr_scale < min
            ? min
            : this.curr_scale > max
            ? max
            : this.curr_scale,
      });
      this.props.onSizeChange({
        width: this.setEditorRef.current.getImage().width,
        height: this.setEditorRef.current.getImage().height,
      });
    }
  };

  onTouchEnd = (evt) => {
    var tt = evt.targetTouches;
    if (tt.length < 2) {
      this.scaling = false;
      if (this.curr_scale < 1) {
        this.scale_factor = 1;
      } else {
        if (this.curr_scale > 10) {
          this.scale_factor = 10;
        } else {
          this.scale_factor = this.curr_scale;
        }
      }
    } else {
      this.scaling = true;
    }
  };

  onWheel = (e) => {
    if (this.props.isLoading) return;
    if (!this.setEditorRef.current) return;
    e = e || window.event;
    const delta = e.deltaY || e.detail || e.wheelDelta;
    let scale =
      delta > 0 && this.state.scale === 1
        ? 1
        : this.state.scale - (delta / 100) * 0.1;
    scale = Math.round(scale * 10) / 10;
    this.setState({
      scale: scale < 1 ? 1 : scale > 10 ? 10 : scale,
    });
    this.props.onSizeChange({
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height,
    });
  };

  onRotateLeftClick = (e) => {
    e.preventDefault();
    this.setState({
      rotate: this.state.rotate - 90,
    });
  };

  onRotateRightClick = (e) => {
    e.preventDefault();
    this.setState({
      rotate: this.state.rotate + 90,
    });
  };

  onFlipVerticalClick = () => {};

  onFlipHorizontalClick = () => {};

  onZoomMinusClick = () => {
    if (this.props.isLoading) return;
    const newScale = this.state.scale - step;
    this.setState({ scale: newScale < min ? min : newScale });
  };

  onZoomPlusClick = () => {
    if (this.props.isLoading) return;
    const newScale = this.state.scale + step;
    this.setState({ scale: newScale > max ? max : newScale });
  };

  handleScale = (e) => {
    if (this.props.isLoading) return;
    const scale = parseFloat(e.target.value);
    this.setState({ scale });
    this.props.onSizeChange({
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height,
    });
  };

  onImageReady = () => {
    this.setState({
      croppedImage: this.setEditorRef.current.getImage().toDataURL(),
    });
    this.props.onImageChange(this.setEditorRef.current.getImage().toDataURL());
    this.props.onPositionChange({
      x: 0.5,
      y: 0.5,
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height,
    });
  };

  onSaveImage() {
    var img = new Image();
    var _this = this;
    img.crossOrigin = "Anonymous";
    img.src = this.state.image;
    if (!this.state.image) _this.props.onLoadFile(null);
    img.onload = () => {
      var canvas = document.createElement("canvas");
      canvas.setAttribute("width", img.width);
      canvas.setAttribute("height", img.height);
      var context = canvas.getContext("2d");

      context.translate(canvas.width / 2, canvas.height / 2);
      context.rotate((this.state.rotate * Math.PI) / 180);
      context.drawImage(img, -img.width / 2, -img.height / 2);

      var rotatedImageSrc = canvas.toDataURL("image/jpeg");
      fetch(rotatedImageSrc)
        .then((res) => res.blob())
        .then((blob) => {
          const file = new File([blob], "File name", { type: "image/jpg" });
          _this.props.onLoadFile(file, true);
        });
    };
  }

  componentDidUpdate(prevProps) {
    if (
      prevProps.image !== this.props.image ||
      prevProps.visible !== this.props.visible
    ) {
      this.setState({
        image: this.props.image ? this.props.image : "",
        rotate: 0,
      });
    }
  }

  openDialog = () => {
    if (!this.state.image) return;
    // Note that the ref is set async,
    // so it might be null at some point
    if (this.dropzoneRef.current) {
      this.dropzoneRef.current.open();
    }
  };

  renderLinkContainer = () => {
    const {
      selectNewPhotoLabel,
      orDropFileHereLabel,
      maxSizeLabel,
      isLoading,
    } = this.props;
    const { image } = this.state;

    const desktopMode = isDesktop();
    const labelAlign = image === "" ? "center" : "left";

    //console.log("maxSizeLabel", maxSizeLabel);
    const onClickProp = !isLoading ? { onClick: this.openDialog } : {};

    return (
      <Text as="span" textAlign={!desktopMode ? labelAlign : "left"}>
        <Link type="action" fontWeight={600} isHovered {...onClickProp}>
          {selectNewPhotoLabel}
        </Link>{" "}
        {desktopMode && orDropFileHereLabel}
        <Text
          as="p"
          // color="#A3A9AE"
          fontSize="12px"
          fontWeight="600"
          textAlign={labelAlign}
        >
          {maxSizeLabel}
        </Text>
      </Text>
    );
  };

  render() {
    const {
      maxSize,
      accept,
      role,
      title,
      useModalDialog,
      isLoading,
    } = this.props;

    const desktopMode = isDesktop();
    //const tabletMode = isTablet();
    //const mobileMode = isMobile();

    let editorWidth = 174;
    let editorHeight = 174;

    if (!useModalDialog) {
      editorWidth = 270;
      editorHeight = 270;
    }

    /*if (tabletMode) {
      editorWidth = 320;
      editorHeight = 320;
    } else if (mobileMode) {
      editorWidth = 287;
      editorHeight = 287;
    }*/

    const onDeleteProp = !isLoading ? { onClick: this.deleteImage } : {};

    return (
      <StyledAvatarEditorBody
        onWheel={this.onWheel}
        onTouchStart={this.onTouchStart}
        onTouchMove={this.onTouchMove}
        onTouchEnd={this.onTouchEnd}
        useModalDialog={useModalDialog}
        image={this.state.image}
      >
        <Dropzone
          ref={this.dropzoneRef}
          onDropAccepted={this.onDropAccepted}
          onDropRejected={this.onDropRejected}
          maxSize={maxSize}
          accept={accept}
          noClick={this.state.image !== ""}
        >
          {({ getRootProps, getInputProps }) => (
            <DropZoneContainer {...getRootProps()}>
              <input {...getInputProps()} />
              {this.state.image === "" ? (
                <Box className="dropzone-text">
                  {this.renderLinkContainer()}
                </Box>
              ) : (
                <StyledAvatarContainer useModalDialog={useModalDialog}>
                  <Box className="preview-container">
                    <Box className="editor-container">
                      <ReactAvatarEditor
                        ref={this.setEditorRef}
                        width={editorWidth}
                        height={editorHeight}
                        borderRadius={200}
                        scale={this.state.scale}
                        className="react-avatar-editor"
                        image={this.state.image}
                        rotate={this.state.rotate}
                        color={[196, 196, 196, 0.5]}
                        onImageChange={this.throttledSetCroppedImage}
                        onPositionChange={this.onPositionChange}
                        onImageReady={this.onImageReady}
                        crossOrigin="anonymous"
                      />
                      <Box className="editor-buttons">
                        <Box></Box>
                        <Box></Box>
                        <Box></Box>
                        <Box></Box>
                        <Box></Box>
                        <IconButton
                          size="16"
                          isDisabled={isLoading}
                          {...onDeleteProp}
                          iconName={"/static/images/catalog.trash.react.svg"}
                          isFill={true}
                          isClickable={true}
                          className="editor-button"
                        />
                      </Box>
                      <Box className="zoom-container">
                        <IconButton
                          className="zoom-container-svg_zoom-minus"
                          size="16"
                          isDisabled={isLoading}
                          onClick={this.onZoomMinusClick}
                          iconName={"/static/images/zoom-minus.react.svg"}
                          isFill={true}
                          isClickable={false}
                        />
                        <Slider
                          id="scale"
                          type="range"
                          className="custom-range"
                          min={this.state.allowZoomOut ? "0.1" : min}
                          max={max}
                          step={step}
                          value={this.state.scale}
                          onChange={this.handleScale}
                        />
                        <IconButton
                          size="16"
                          className="zoom-container-svg_zoom-plus"
                          isDisabled={isLoading}
                          onClick={this.onZoomPlusClick}
                          iconName={"/static/images/zoom-plus.react.svg"}
                          isFill={true}
                          isClickable={false}
                        />
                      </Box>
                    </Box>
                    {desktopMode && useModalDialog && (
                      <Box className="avatar-container">
                        <Avatar
                          size="max"
                          role={role}
                          source={this.state.croppedImage}
                          editing={false}
                        />
                        <Box className="avatar-mini-preview">
                          <Avatar
                            size="min"
                            role={role}
                            source={this.state.croppedImage}
                            editing={false}
                          />
                          <Text
                            as="div"
                            fontSize="15px"
                            fontWeight={600}
                            title={title}
                            truncate={true}
                          >
                            {title}
                          </Text>
                        </Box>
                      </Box>
                    )}
                  </Box>
                  <Box className="link-container">
                    {this.renderLinkContainer()}
                  </Box>
                </StyledAvatarContainer>
              )}
            </DropZoneContainer>
          )}
        </Dropzone>

        <StyledErrorContainer key="errorMsg">
          {this.state.errorText !== null && (
            <Text as="p" color="#C96C27" isBold={true}>
              {this.state.errorText}
            </Text>
          )}
        </StyledErrorContainer>
      </StyledAvatarEditorBody>
    );
  }
}

AvatarEditorBody.propTypes = {
  onImageChange: PropTypes.func,
  onPositionChange: PropTypes.func,
  onSizeChange: PropTypes.func,
  visible: PropTypes.bool,
  onLoadFileError: PropTypes.func,
  onLoadFile: PropTypes.func,
  deleteImage: PropTypes.func,
  maxSize: PropTypes.number,
  image: PropTypes.string,
  accept: PropTypes.arrayOf(PropTypes.string),
  selectNewPhotoLabel: PropTypes.string,
  orDropFileHereLabel: PropTypes.string,
  unknownTypeError: PropTypes.string,
  maxSizeFileError: PropTypes.string,
  unknownError: PropTypes.string,
  role: PropTypes.string,
  title: PropTypes.string,
  useModalDialog: PropTypes.bool,
  maxSizeLabel: PropTypes.string,
  isLoading: PropTypes.bool,
};

AvatarEditorBody.defaultProps = {
  accept: ["image/png", "image/jpeg"],
  maxSize: Number.MAX_SAFE_INTEGER,
  visible: false,
  selectNewPhotoLabel: "Select new photo",
  orDropFileHereLabel: "or drop file here",
  unknownTypeError: "Unknown image file type",
  maxSizeFileError: "Maximum file size exceeded",
  unknownError: "Error",
  role: "user",
  title: "Sample title",
  useModalDialog: true,
  isLoading: false,
};
export default AvatarEditorBody;
