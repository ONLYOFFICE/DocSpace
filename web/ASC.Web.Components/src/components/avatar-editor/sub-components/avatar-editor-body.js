import React from "react";
import styled from "styled-components";
import Dropzone from "react-dropzone";
import ReactAvatarEditor from "react-avatar-editor";
import PropTypes from "prop-types";
import Avatar from "../../avatar/index";
import accepts from "attr-accept";
import Text from "../../text";
import Box from "../../box";
import Row from "../../row";
import IconButton from "../../icon-button";

import { tablet } from "../../../utils/device";
import resizeImage from "resize-image";

const step = 0.01;
const min = 1;
const max = 5;

const StyledErrorContainer = styled.div`
  p {
    text-align: center;
  }
`;

const Slider = styled.input.attrs({
  type: "range"
})`
  width: 100%;
  margin: 8px 0;
  background-color: transparent;
  -webkit-appearance: none;

  &:focus {
    outline: none;
  }

  &::-webkit-slider-runnable-track {
    background: #eceef1;
    border: 1.4px solid #eceef1;
    border-radius: 5.6px;
    width: 100%;
    height: 8px;
    cursor: pointer;
  }

  &::-webkit-slider-thumb {
    margin-top: -9.4px;
    width: 24px;
    height: 24px;
    background: #2da7db;
    border: 6px solid #ffffff;
    border-radius: 30px;
    cursor: pointer;
    -webkit-appearance: none;
  }

  &:focus::-webkit-slider-runnable-track {
    background: #eceef1;
  }

  &::-moz-range-track {
    background: #eceef1;
    border: 1.4px solid #eceef1;
    border-radius: 5.6px;
    width: 100%;
    height: 8px;
    cursor: pointer;
  }

  &::-moz-range-thumb {
    width: 24px;
    height: 24px;
    background: #2da7db;
    border: 6px solid #ffffff;
    border-radius: 30px;
    cursor: pointer;
  }

  &::-ms-track {
    background: transparent;
    border-color: transparent;
    border-width: 10.2px 0;
    color: transparent;
    width: 100%;
    height: 8px;
    cursor: pointer;
  }

  &::-ms-fill-lower {
    background: #eceef1;
    border: 1.4px solid #eceef1;
    border-radius: 11.2px;
  }

  &::-ms-fill-upper {
    background: #eceef1;
    border: 1.4px solid #eceef1;
    border-radius: 11.2px;
  }

  &::-ms-thumb {
    width: 24px;
    height: 24px;
    background: #2da7db;
    border: 6px solid #ffffff;
    border-radius: 30px;
    cursor: pointer;
    margin-top: 0px;
    /*Needed to keep the Edge thumb centred*/
  }

  &:focus::-ms-fill-lower {
    background: #eceef1;
  }

  &:focus::-ms-fill-upper {
    background: #eceef1;
  }
`;

// const CloseButton = styled.a`
//   cursor: pointer;
//   position: absolute;
//   right: 33px;
//   top: 4px;
//   width: 16px;
//   height: 16px;

//   &:before,
//   &:after {
//     position: absolute;
//     left: 8px;
//     content: " ";
//     height: 16px;
//     width: 1px;
//     background-color: #d8d8d8;
//   }
//   &:before {
//     transform: rotate(45deg);
//   }
//   &:after {
//     transform: rotate(-45deg);
//   }
//   @media ${tablet} {
//     right: calc(50% - 147px);
//   }
// `;

const DropZoneContainer = styled.div`
  box-sizing: border-box;
  display: block;
  width: 100%;
  height: 300px;
  border: 1px dashed #ccc;
  text-align: center;
  padding: 10em 0;
  margin: 0 auto;
  p {
    margin: 0;
    cursor: default;
  }
  .desktop {
    display: block;
  }
  .mobile {
    display: none;
  }
  @media ${tablet} {
    .desktop {
      display: none;
    }
    .mobile {
      display: block;
    }
  }
`;

const StyledAvatarContainer = styled.div`
  text-align: center;

  display: grid;
  grid-template-columns: min-content 1fr;
  grid-column-gap: 16px;

  .custom-range {
    width: 100%;
    display: block;
  }
  .avatar-container {
    width: 160px;
    display: grid;
    grid-template-rows: 160px 48px;
    grid-row-gap: 16px;

    .avatar-mini-preview {
      width: 160px;

      border: 1px solid #ECEEF1;
      box-sizing: border-box;
      border-radius: 6px;

      .avatar-mini-preview__row {
        border-bottom: none;

        .row_content {
          min-width: 40px;
        }

        .row_context-menu-wrapper {
          padding-right: 8px;
        }
      }
    }

    /* display: inline-block;
    vertical-align: top;
    @media ${tablet} {
      display: none;
    } */
  }
  .editor-container {
    width: 224px;
    display: grid;

    @media ${tablet} {
      padding: 0;
    }

    .react-avatar-editor {
    }

    .editor-buttons {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      grid-template-rows: 32px;
      height: 32px;
      background: #a3a9ae;
      justify-items: center;
    }

    .zoom-container {
      height: 56px;
      display: grid;
      grid-template-columns: min-content 1fr min-content;
      grid-column-gap: 12px;
    }
  }
`;
// const StyledASCAvatar = styled(ASCAvatar)`
//   display: block;
//   @media ${tablet} {
//     display: none;
//   }
// `;

class AvatarEditorBody extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      image: this.props.image ? this.props.image : "",
      scale: 1,
      croppedImage: "",
      errorText: null,
      rotate: 0
    };

    this.setEditorRef = React.createRef();
  }

  onPositionChange = position => {
    this.props.onPositionChange({
      x: position.x,
      y: position.y,
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height
    });
  };

  onDropRejected = rejectedFiles => {
    if (!accepts(rejectedFiles[0], this.props.accept)) {
      this.props.onLoadFileError(0);
      this.setState({
        errorText: this.props.unknownTypeError
      });
      return;
    } else if (rejectedFiles[0].size > this.props.maxSize) {
      this.props.onLoadFileError(1);
      this.setState({
        errorText: this.props.maxSizeFileError
      });
      return;
    }
    this.setState({
      errorText: this.props.unknownError
    });
    this.props.onLoadFileError(2);
  };

  onDropAccepted = acceptedFiles => {
    const _this = this;
    var fr = new FileReader();
    fr.readAsDataURL(acceptedFiles[0]);
    fr.onload = function() {
      var img = new Image();
      img.onload = function() {
        var canvas = resizeImage.resize2Canvas(img, 1024, 1024);
        var data = resizeImage.resize(canvas, 1024, 1024, resizeImage.JPEG);
        _this.setState({
          image: data,
          rotate: 0,
          errorText: null
        });
        fetch(data)
          .then(res => res.blob())
          .then(blob => {
            const file = new File([blob], "File name", { type: "image/jpg" });
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
      croppedImage: ""
    });
    this.props.deleteImage();
  };

  onImageChange = () => {
    const image = this.setEditorRef.current.getImage().toDataURL();
    if (this.setEditorRef.current !== null) {
      this.setState({
        croppedImage: image
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

  onTouchStart = evt => {
    evt.preventDefault();
    var tt = evt.targetTouches;
    if (tt.length >= 2) {
      this.dist = this.distance(tt[0], tt[1]);
      this.scaling = true;
    } else {
      this.scaling = false;
    }
  };

  onTouchMove = evt => {
    evt.preventDefault();
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
            : this.curr_scale
      });
      this.props.onSizeChange({
        width: this.setEditorRef.current.getImage().width,
        height: this.setEditorRef.current.getImage().height
      });
    }
  };

  onTouchEnd = evt => {
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

  onWheel = e => {
    e = e || window.event;
    const delta = e.deltaY || e.detail || e.wheelDelta;
    let scale =
      delta > 0 && this.state.scale === 1
        ? 1
        : this.state.scale - (delta / 100) * 0.1;
    scale = Math.round(scale * 10) / 10;
    this.setState({
      scale: scale < 1 ? 1 : scale > 10 ? 10 : scale
    });
    this.props.onSizeChange({
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height
    });
  };

  onRotateLeftClick = e => {
    e.preventDefault();
    this.setState({
      rotate: this.state.rotate - 90
    });
  };

  onRotateRightClick = e => {
    e.preventDefault();
    this.setState({
      rotate: this.state.rotate + 90
    });
  };

  onFlipVerticalClick = () => {};

  onFlipHorizontalClick = () => {};

  onZoomMinusClick = () => {
    const newScale = this.state.scale - step;
    this.setState({ scale: newScale < min ? min : newScale });
  };

  onZoomPlusClick = () => {
    const newScale = this.state.scale + step;
    this.setState({ scale: newScale > max ? max : newScale });
  };

  handleScale = e => {
    const scale = parseFloat(e.target.value);
    this.setState({ scale });
    this.props.onSizeChange({
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height
    });
  };

  onImageReady = () => {
    this.setState({
      croppedImage: this.setEditorRef.current.getImage().toDataURL()
    });
    this.props.onImageChange(this.setEditorRef.current.getImage().toDataURL());
    this.props.onPositionChange({
      x: 0.5,
      y: 0.5,
      width: this.setEditorRef.current.getImage().width,
      height: this.setEditorRef.current.getImage().height
    });
  };

  onSaveImage(callback) {
    var img = new Image();
    var _this = this;
    img.src = this.state.image;
    img.onload = () => {
      var canvas = document.createElement("canvas");
      canvas.setAttribute("width", img.height);
      canvas.setAttribute("height", img.width);
      var context = canvas.getContext("2d");

      context.translate(canvas.width / 2, canvas.height / 2);
      context.rotate((this.state.rotate * Math.PI) / 180);
      context.drawImage(img, -img.width / 2, -img.height / 2);

      var rotatedImageSrc = canvas.toDataURL("image/jpeg");
      fetch(rotatedImageSrc)
        .then(res => res.blob())
        .then(blob => {
          const file = new File([blob], "File name", { type: "image/jpg" });
          _this.props.onLoadFile(file, callback);
        });
    };
  }

  componentDidUpdate(prevProps) {
    if (prevProps.image !== this.props.image) {
      setTimeout(() => {
        this.setState({
          image: this.props.image,
          rotate: 0
        });
      }, 0);
    } else if (!prevProps.visible && this.props.visible) {
      this.setState({
        image: this.props.image ? this.props.image : "",
        rotate: 0
      });
    }
  }

  render() {
    const {
      maxSize,
      accept,
      chooseFileLabel,
      chooseMobileFileLabel,
      role,
      title
    } = this.props;

    return (
      <div
        onWheel={this.onWheel}
        onTouchStart={this.onTouchStart}
        onTouchMove={this.onTouchMove}
        onTouchEnd={this.onTouchEnd}
      >
        {this.state.image === "" ? (
          <Dropzone
            onDropAccepted={this.onDropAccepted}
            onDropRejected={this.onDropRejected}
            maxSize={maxSize}
            accept={accept}
          >
            {({ getRootProps, getInputProps }) => (
              <DropZoneContainer {...getRootProps()}>
                <input {...getInputProps()} />
                <p className="desktop">{chooseFileLabel}</p>
                <p className="mobile">{chooseMobileFileLabel}</p>
              </DropZoneContainer>
            )}
          </Dropzone>
        ) : (
          <StyledAvatarContainer>
            <Box className="editor-container">
              <ReactAvatarEditor
                ref={this.setEditorRef}
                width={174}
                height={174}
                borderRadius={200}
                scale={this.state.scale}
                className="react-avatar-editor"
                image={this.state.image}
                rotate={this.state.rotate}
                color={[0, 0, 0, 0.5]}
                onImageChange={this.onImageChange}
                onPositionChange={this.onPositionChange}
                onImageReady={this.onImageReady}
              />
              <Box className="editor-buttons">
                <IconButton
                  size="16"
                  isDisabled={false}
                  onClick={this.onRotateLeftClick}
                  iconName={"RotateLeftIcon"}
                  isFill={true}
                  isClickable={false}
                  color="#FFFFFF"
                />
                <IconButton
                  size="16"
                  isDisabled={false}
                  onClick={this.onRotateRightClick}
                  iconName={"RotateRightIcon"}
                  isFill={true}
                  isClickable={false}
                  color="#FFFFFF"
                />
                <IconButton
                  size="16"
                  isDisabled={true}
                  onClick={this.onFlipVerticalClick}
                  iconName={"FlipVerticalIcon"}
                  isFill={true}
                  isClickable={false}
                  color="#FFFFFF"
                />
                <IconButton
                  size="16"
                  isDisabled={true}
                  onClick={this.onFlipHorizontalClick}
                  iconName={"FlipHorizontalIcon"}
                  isFill={true}
                  isClickable={false}
                  color="#FFFFFF"
                />
              </Box>
              <Box className="zoom-container">
                <IconButton
                  size="16"
                  isDisabled={false}
                  onClick={this.onZoomMinusClick}
                  iconName={"ZoomMinusIcon"}
                  isFill={true}
                  isClickable={false}
                />
                <Slider
                  id="scale"
                  type="range"
                  className="custom-range"
                  onChange={this.handleScale}
                  min={this.state.allowZoomOut ? "0.1" : min}
                  max={max}
                  step={step}
                  value={this.state.scale}
                />
                <IconButton
                  size="16"
                  isDisabled={false}
                  onClick={this.onZoomPlusClick}
                  iconName={"ZoomPlusIcon"}
                  isFill={true}
                  isClickable={false}
                />
              </Box>
              {/* <CloseButton onClick={this.deleteImage}></CloseButton> */}
            </Box>
            <Box className="avatar-container">
              <Avatar
                size="max"
                role={role}
                source={this.state.croppedImage}
                editing={false}
              />
              <Box className="avatar-mini-preview">
                <Row
                  className="avatar-mini-preview__row"
                  key="test-avatar"
                  element={
                    <Avatar
                      size="small"
                      role={role}
                      source={this.state.croppedImage}
                      editing={false}
                    />
                  }
                  contextOptions={[
                    {
                      key: "send-email",
                      label: "Send email",
                      onClick: () => alert("Hello world!")
                    }
                  ]}
                >
                  <Text
                    as="div"
                    fontSize="15px"
                    fontWeight={600}
                    title={title}
                    truncate={true}
                  >
                    {title}
                  </Text>
                </Row>
              </Box>
            </Box>
          </StyledAvatarContainer>
        )}
        <StyledErrorContainer key="errorMsg">
          {this.state.errorText !== null && (
            <Text as="p" color="#C96C27" isBold={true}>
              {this.state.errorText}
            </Text>
          )}
        </StyledErrorContainer>
      </div>
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
  chooseFileLabel: PropTypes.string,
  chooseMobileFileLabel: PropTypes.string,
  unknownTypeError: PropTypes.string,
  maxSizeFileError: PropTypes.string,
  unknownError: PropTypes.string,
  role: PropTypes.string,
  title: PropTypes.string
};

AvatarEditorBody.defaultProps = {
  accept: ["image/png", "image/jpeg"],
  maxSize: Number.MAX_SAFE_INTEGER,
  visible: false,
  chooseFileLabel: "Drop files here, or click to select files",
  chooseMobileFileLabel: "Click to select files",
  unknownTypeError: "Unknown image file type",
  maxSizeFileError: "Maximum file size exceeded",
  unknownError: "Error",
  role: "user",
  title: "Sample title"
};
export default AvatarEditorBody;
