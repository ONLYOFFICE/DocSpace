import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "../modal-dialog";
import Button from "../button";
import AvatarEditorBody from "./sub-components/avatar-editor-body";
import StyledButtonsWrapper from "./styled-avatar-editor";

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
    this.props.onSave && this.props.onSave();
    this.avatarEditorBodyRef.current.onSaveImage();
  };

  onCancelButtonClick = () => {
    this.props.onCancel();
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
      maxSizeLabel,
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
            maxSize={maxSize * 1024 * 1024} // megabytes to bytes
            accept={accept}
            image={image}
            selectNewPhotoLabel={selectNewPhotoLabel}
            orDropFileHereLabel={orDropFileHereLabel}
            unknownTypeError={unknownTypeError}
            maxSizeFileError={maxSizeFileError}
            unknownError={unknownError}
            maxSizeLabel={maxSizeLabel}
            isLoading={saveButtonLoading}
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
          maxSize={maxSize * 1000000} // megabytes to bytes
          accept={accept}
          image={image}
          selectNewPhotoLabel={selectNewPhotoLabel}
          orDropFileHereLabel={orDropFileHereLabel}
          unknownTypeError={unknownTypeError}
          maxSizeFileError={maxSizeFileError}
          unknownError={unknownError}
          useModalDialog={false}
          maxSizeLabel={maxSizeLabel}
          isLoading={saveButtonLoading}
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
  /** Display avatar editor */
  visible: PropTypes.bool,
  /** Translation string for title */
  headerLabel: PropTypes.string,
  /** Translation string for file selection */
  selectNewPhotoLabel: PropTypes.string,
  /** Translation string for file dropping (concat with selectNewPhotoLabel prop) */
  orDropFileHereLabel: PropTypes.string,
  /** Translation string for save button */
  saveButtonLabel: PropTypes.string,
  /** Translation string for cancel button */
  cancelButtonLabel: PropTypes.string,
  /** Tells when the button should show loader icon */
  saveButtonLoading: PropTypes.bool,
  /** Translation string for size warning */
  maxSizeFileError: PropTypes.string,
  /** Display avatar editor */
  image: PropTypes.string,
  /** Max size of image */
  maxSize: PropTypes.number,
  /** Accepted file types */
  accept: PropTypes.arrayOf(PropTypes.string),
  /** Save event */
  onSave: PropTypes.func,
  /** Closing event */
  onClose: PropTypes.func,
  /** Image deletion event */
  onDeleteImage: PropTypes.func,
  /** Image upload event */
  onLoadFile: PropTypes.func,
  /** Image change event */
  onImageChange: PropTypes.func,
  /** Translation string for load file warning */
  onLoadFileError: PropTypes.func,
  /** Translation string for file type warning */
  unknownTypeError: PropTypes.string,
  /** Translation string for warning */
  unknownError: PropTypes.string,
  /** Display type */
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  /** Accepts class" */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Use for the view of the modal dialog or not */
  useModalDialog: PropTypes.bool,
  maxSizeLabel: PropTypes.string,
};

AvatarEditor.defaultProps = {
  visible: false,
  maxSize: 25,
  headerLabel: "Edit Photo",
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
  accept: ["image/png", "image/jpeg"],
  displayType: "auto",
  useModalDialog: true,
};

export default AvatarEditor;
