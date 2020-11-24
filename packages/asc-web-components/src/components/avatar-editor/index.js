import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "../modal-dialog";
import Button from "../button";
import AvatarEditorBody from "./sub-components/avatar-editor-body";
import styled from "styled-components";

const StyledButtonsWrapper = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 8px;
  min-width: 208px;
  max-width: 300px;
  width: max-content;
`;

class AvatarEditor extends React.Component {
  constructor(props) {
    super(props);
    this.avatarEditorBodyRef = React.createRef();

    this.state = {
      existImage: !!this.props.image,
      visible: props.visible,
      x: 0,
      y: 0,
      width: 0,
      height: 0,
      croppedImage: "",
    };
  }

  onImageChange = (file) => {
    this.setState({
      croppedImage: file,
    });
    if (typeof this.props.onImageChange === "function")
      this.props.onImageChange(file);
  };

  onDeleteImage = () => {
    this.setState({
      existImage: false,
    });
    if (typeof this.props.onDeleteImage === "function")
      this.props.onDeleteImage();
  };

  onSizeChange = (data) => {
    this.setState(data);
  };

  onPositionChange = (data) => {
    this.setState(data);
  };

  onLoadFileError = (error) => {
    if (typeof this.props.onLoadFileError === "function")
      this.props.onLoadFileError(error);
  };

  onLoadFile = (file, needSave) => {
    if (typeof this.props.onLoadFile === "function") {
      var fileData = {
        existImage: this.state.existImage,
        position: {
          x: this.state.x,
          y: this.state.y,
          width: this.state.width,
          height: this.state.height,
        },
        croppedImage: this.state.croppedImage,
      };

      needSave
        ? this.props.onLoadFile(file, fileData)
        : this.props.onLoadFile(file);
    }

    if (!this.state.existImage) this.setState({ existImage: true });
  };

  onSaveButtonClick = () => {
    this.avatarEditorBodyRef.current.onSaveImage();
    //this.saveAvatar();
  };

  onCancelButtonClick = () => {
    this.props.onCancel();
  };

  saveAvatar = () => {
    if (!this.state.existImage) {
      this.props.onSave(this.state.existImage);
      return;
    }

    this.props.onSave(
      this.state.existImage,
      {
        x: this.state.x,
        y: this.state.y,
        width: this.state.width,
        height: this.state.height,
      },
      this.state.croppedImage
    );
  };

  onClose = () => {
    this.setState({ visible: false });
    this.props.onClose();
  };

  componentDidUpdate(prevProps) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ visible: this.props.visible });
    }
    if (this.props.image !== prevProps.image) {
      this.setState({ existImage: !!this.props.image });
    }
  }

  render() {
    const {
      displayType,
      className,
      id,
      style,
      headerLabel,
      maxSize,
      accept,
      image,
      selectNewPhotoLabel,
      orDropFileHereLabel,
      unknownTypeError,
      maxSizeFileError,
      unknownError,
      saveButtonLabel,
      saveButtonLoading,
      useModalDialog,
      cancelButtonLabel,
    } = this.props;

    return useModalDialog ? (
      <ModalDialog
        visible={this.state.visible}
        displayType={displayType}
        scale={false}
        contentHeight="initial"
        contentWidth="initial"
        onClose={this.onClose}
        className={className}
        id={id}
        style={style}
      >
        <ModalDialog.Header>{headerLabel}</ModalDialog.Header>
        <ModalDialog.Body>
          <AvatarEditorBody
            ref={this.avatarEditorBodyRef}
            visible={this.state.visible}
            onImageChange={this.onImageChange}
            onPositionChange={this.onPositionChange}
            onSizeChange={this.onSizeChange}
            onLoadFileError={this.onLoadFileError}
            onLoadFile={this.onLoadFile}
            deleteImage={this.onDeleteImage}
            saveAvatar={this.saveAvatar}
            maxSize={maxSize * 1000000} // megabytes to bytes
            accept={accept}
            image={image}
            selectNewPhotoLabel={selectNewPhotoLabel}
            orDropFileHereLabel={orDropFileHereLabel}
            unknownTypeError={unknownTypeError}
            maxSizeFileError={maxSizeFileError}
            unknownError={unknownError}
          />
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SaveBtn"
            label={saveButtonLabel}
            isLoading={saveButtonLoading}
            primary={true}
            size="big"
            onClick={this.onSaveButtonClick}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    ) : (
      <>
        <AvatarEditorBody
          ref={this.avatarEditorBodyRef}
          visible={this.state.visible}
          onImageChange={this.onImageChange}
          onPositionChange={this.onPositionChange}
          onSizeChange={this.onSizeChange}
          onLoadFileError={this.onLoadFileError}
          onLoadFile={this.onLoadFile}
          deleteImage={this.onDeleteImage}
          saveAvatar={this.saveAvatar}
          maxSize={maxSize * 1000000} // megabytes to bytes
          accept={accept}
          image={image}
          selectNewPhotoLabel={selectNewPhotoLabel}
          orDropFileHereLabel={orDropFileHereLabel}
          unknownTypeError={unknownTypeError}
          maxSizeFileError={maxSizeFileError}
          unknownError={unknownError}
          useModalDialog={false}
        />
        <StyledButtonsWrapper>
          <Button
            key="SaveBtn"
            label={saveButtonLabel}
            isLoading={saveButtonLoading}
            primary={true}
            size="big"
            onClick={this.onSaveButtonClick}
          />
          <Button
            key="CancelBtn"
            label={cancelButtonLabel}
            primary={false}
            size="big"
            onClick={this.onCancelButtonClick}
          />
        </StyledButtonsWrapper>
      </>
    );
  }
}

AvatarEditor.propTypes = {
  visible: PropTypes.bool,
  headerLabel: PropTypes.string,
  selectNewPhotoLabel: PropTypes.string,
  orDropFileHereLabel: PropTypes.string,

  saveButtonLabel: PropTypes.string,
  cancelButtonLabel: PropTypes.string,
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
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  useModalDialog: PropTypes.bool,
};

AvatarEditor.defaultProps = {
  visible: false,
  maxSize: 10, //10MB
  headerLabel: "Edit Photo",
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
  accept: ["image/png", "image/jpeg"],
  displayType: "auto",
  useModalDialog: true,
};

export default AvatarEditor;
