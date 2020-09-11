import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "../modal-dialog";
import Button from "../button";
import IconButton from "../icon-button";
import AvatarEditorBody from "./sub-components/avatar-editor-body";

class AvatarEditor extends React.Component {
  constructor(props) {
    super(props);
    this.avatarEditorBodyRef = React.createRef();

    this.state = {
      isContainsFile: !!this.props.image,
      visible: props.visible,
      x: 0,
      y: 0,
      width: 0,
      height: 0,
      croppedImage: ""
    };

    this.onClose = this.onClose.bind(this);
    this.onSaveButtonClick = this.onSaveButtonClick.bind(this);
    this.onImageChange = this.onImageChange.bind(this);
    this.onLoadFileError = this.onLoadFileError.bind(this);
    this.onLoadFile = this.onLoadFile.bind(this);
    this.onPositionChange = this.onPositionChange.bind(this);
    this.onSizeChange = this.onSizeChange.bind(this);

    this.onDeleteImage = this.onDeleteImage.bind(this);
  }

  onImageChange(file) {
    this.setState({
      croppedImage: file
    });
    if (typeof this.props.onImageChange === "function")
      this.props.onImageChange(file);
  }
  onDeleteImage() {
    this.setState({
      isContainsFile: false
    });
    if (typeof this.props.onDeleteImage === "function")
      this.props.onDeleteImage();
  }
  onSizeChange(data) {
    this.setState(data);
  }
  onPositionChange(data) {
    this.setState(data);
  }
  onLoadFileError(error) {
    if (typeof this.props.onLoadFileError === "function")
      this.props.onLoadFileError(error);
  }
  onLoadFile(file, callback) {
    if (typeof this.props.onLoadFile === "function")
      this.props.onLoadFile(file, callback);
    if (!this.state.isContainsFile) this.setState({ isContainsFile: true });
  }
  onSaveButtonClick() {
    this.avatarEditorBodyRef.current.onSaveImage(this.saveAvatar);
  }
  saveAvatar = () => {
    this.state.isContainsFile
      ? this.props.onSave(
        this.state.isContainsFile,
        {
          x: this.state.x,
          y: this.state.y,
          width: this.state.width,
          height: this.state.height
        },
        this.state.croppedImage
      )
      : this.props.onSave(this.state.isContainsFile);
  };
  onClickRotateLeft = e => {
    this.avatarEditorBodyRef.current.rotateLeft(e);
  };
  onClose() {
    this.setState({ visible: false });
    this.props.onClose();
  }
  componentDidUpdate(prevProps) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ visible: this.props.visible });
    }
    if (this.props.image !== prevProps.image) {
      this.setState({ isContainsFile: !!this.props.image });
    }
  }

  render() {
    return (
      <ModalDialog
        visible={this.state.visible}
        displayType={this.props.displayType}
        scale={true}
        onClose={this.onClose}
        className={this.props.className}
        id={this.props.id}
        style={this.props.style}
      >
        <ModalDialog.Header>{this.props.headerLabel}</ModalDialog.Header>
        <ModalDialog.Body>
          <AvatarEditorBody
            onImageChange={this.onImageChange}
            visible={this.state.visible}
            onPositionChange={this.onPositionChange}
            onSizeChange={this.onSizeChange}
            onLoadFileError={this.onLoadFileError}
            onLoadFile={this.onLoadFile}
            deleteImage={this.onDeleteImage}
            saveAvatar={this.saveAvatar}
            maxSize={this.props.maxSize * 1000000} // megabytes to bytes
            accept={this.props.accept}
            image={this.props.image}
            chooseFileLabel={this.props.chooseFileLabel}
            chooseMobileFileLabel={this.props.chooseMobileFileLabel}
            unknownTypeError={this.props.unknownTypeError}
            maxSizeFileError={this.props.maxSizeFileError}
            unknownError={this.props.unknownError}
            ref={this.avatarEditorBodyRef}
          />
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SaveBtn"
            label={this.props.saveButtonLabel}
            isLoading={this.props.saveButtonLoading}
            primary={true}
            size="medium"
            onClick={this.onSaveButtonClick}
          />
          <IconButton
            key="RotateBtn"
            iconName="RotateIcon"
            color="#A3A9AE"
            size="25"
            hoverColor="#657077"
            isFill={true}
            onClick={this.onClickRotateLeft}
            style={{ display: "inline-block", marginLeft: "8px" }}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

AvatarEditor.propTypes = {
  visible: PropTypes.bool,
  headerLabel: PropTypes.string,
  chooseFileLabel: PropTypes.string,
  chooseMobileFileLabel: PropTypes.string,
  saveButtonLabel: PropTypes.string,
  saveButtonLoading: PropTypes.bool,
  maxSizeFileError: PropTypes.string,
  image: PropTypes.string,
  maxSize: PropTypes.number,
  accept: PropTypes.arrayOf(PropTypes.string),
  onSave: PropTypes.func,
  onClose: PropTypes.func,
  onDeleteImage: PropTypes.func,
  onLoadFile: PropTypes.func,
  onImageChange: PropTypes.func,
  onLoadFileError: PropTypes.func,
  unknownTypeError: PropTypes.string,
  unknownError: PropTypes.string,
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

AvatarEditor.defaultProps = {
  visible: false,
  maxSize: 10, //10MB
  headerLabel: "Edit Photo",
  saveButtonLabel: "Save",
  accept: ["image/png", "image/jpeg"],
  displayType: "auto"
};

export default AvatarEditor;
