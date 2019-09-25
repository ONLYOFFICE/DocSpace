import React, { memo } from 'react'
import PropTypes from 'prop-types'
import ModalDialog from '../modal-dialog'
import Button from '../button'
import AvatarEditorBody from './sub-components/avatar-editor-body'

class AvatarEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            isContainsFile: !!this.props.image,
            visible: props.visible,
            x: 0,
            y: 0,
            width: 0,
            height:0
        }

        this.onClose = this.onClose.bind(this);
        this.onSaveButtonClick = this.onSaveButtonClick.bind(this);
        this.onImageChange = this.onImageChange.bind(this);
        this.onLoadFileError = this.onLoadFileError.bind(this);
        this.onLoadFile = this.onLoadFile.bind(this);
        this.onPositionChange = this.onPositionChange.bind(this);

        this.onDeleteImage = this.onDeleteImage.bind(this)
  
    }
    onImageChange(file){
        if(typeof this.props.onImageChange === 'function') this.props.onImageChange(file);
    }
    onDeleteImage(){
        this.setState({
            isContainsFile: false
        })
        if(typeof this.props.onDeleteImage === 'function') this.props.onDeleteImage();
    }
    onPositionChange(data){
        this.setState(data);
    }
    onLoadFileError(error) {
        switch (error) {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }
    }
    onLoadFile(file) {
        if(typeof this.props.onLoadFile === 'function') this.props.onLoadFile(file);
        this.setState({ isContainsFile: true });
    }
    
    onSaveButtonClick() {
        this.props.onSave(this.state.isContainsFile, {
            x: this.state.x,
            y: this.state.y,
            width: this.state.width,
            height: this.state.height
        });
    }

    onClose() {
        this.setState({ visible: false });
        this.props.onClose();
    }

    componentDidUpdate(prevProps) {
        if (this.props.visible !== prevProps.visible) {
            this.setState({ visible: this.props.visible });
        }
    }
    render() {
        return (
            <ModalDialog
                visible={this.state.visible}
                headerContent={this.props.headerLabel}
                bodyContent={
                    <AvatarEditorBody
                        onImageChange={this.onImageChange}
                        onPositionChange={this.onPositionChange}
                        onLoadFileError={this.onLoadFileError}
                        onLoadFile={this.onLoadFile}
                        deleteImage={this.onDeleteImage}
                        maxSize={this.props.maxSize * 1000000} // megabytes to bytes
                        accept={this.props.accept}
                        image={this.props.image}
                        chooseFileLabel={this.props.chooseFileLabel}
                        unknownTypeError={this.props.unknownTypeError}
                        maxSizeFileError={this.props.maxSizeFileError}
                        unknownError={this.props.unknownError}
                    />
                }
                footerContent={[
                    <Button
                        key="SaveBtn"
                        label={this.props.saveButtonLabel}
                        primary={true}
                        onClick={this.onSaveButtonClick}
                    />,
                    <Button
                        key="CancelBtn"
                        label={this.props.cancelButtonLabel}
                        onClick={this.onClose}
                        style={{ marginLeft: "8px" }}
                    />
                ]}
                onClose={this.props.onClose}
            />
        );
    }
}

AvatarEditor.propTypes = {
    visible: PropTypes.bool,
    headerLabel: PropTypes.string,
    chooseFileLabel: PropTypes.string,
    saveButtonLabel: PropTypes.string,
    maxSizeErrorLabel: PropTypes.string,
    image: PropTypes.string,
    cancelButtonLabel: PropTypes.string,
    maxSize: PropTypes.number,
    accept: PropTypes.arrayOf(PropTypes.string),
    onSave: PropTypes.func,
    onClose: PropTypes.func,
    onDeleteImage: PropTypes.func,
    onLoadFile: PropTypes.func,
    onImageChange: PropTypes.func,
    unknownTypeError: PropTypes.string,
    maxSizeFileError: PropTypes.string,
    unknownError: PropTypes.string
};

AvatarEditor.defaultProps = {
    visible: false,
    maxSize: 1, //1MB
    headerLabel: 'Edit Photo',
    saveButtonLabel: 'Save',
    cancelButtonLabel: 'Cancel',
    maxSizeErrorLabel: 'File is too big',
    accept: ['image/png', 'image/jpeg'],
};

export default AvatarEditor;


