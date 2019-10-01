import React from 'react'
import PropTypes from 'prop-types'
import ModalDialog from '../modal-dialog'
import Button from '../button'
import AvatarEditorBody from './sub-components/avatar-editor-body'
import SidePanel from '../side-panel'
import { desktop } from '../../utils/device';
import throttle from 'lodash/throttle';


class AvatarEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            isContainsFile: !!this.props.image,
            visible: props.visible,
            x: 0,
            y: 0,
            width: 0,
            height: 0,
            croppedImage: '',

            displayType: this.props.displayType !== 'auto' ? this.props.displayType : window.innerWidth < desktop.match(/\d+/)[0] ? 'aside' : 'modal'
        }

        this.onClose = this.onClose.bind(this);
        this.onSaveButtonClick = this.onSaveButtonClick.bind(this);
        this.onImageChange = this.onImageChange.bind(this);
        this.onLoadFileError = this.onLoadFileError.bind(this);
        this.onLoadFile = this.onLoadFile.bind(this);
        this.onPositionChange = this.onPositionChange.bind(this);

        this.onDeleteImage = this.onDeleteImage.bind(this);
        this.throttledResize = throttle(this.resize, 300);
        this.popstate = this.popstate.bind(this);
    }
    resize = () => {
        if (this.props.displayType === "auto") {
            const type = window.innerWidth < desktop.match(/\d+/)[0] ? 'aside' : 'modal';
            if (type !== this.state.displayType)
                this.setState({
                    displayType: type
                });
        }
    }
    onImageChange(file) {
        this.setState({
            croppedImage: file
        })
        if (typeof this.props.onImageChange === 'function') this.props.onImageChange(file);
    }
    onDeleteImage() {
        this.setState({
            isContainsFile: false
        })
        if (typeof this.props.onDeleteImage === 'function') this.props.onDeleteImage();
    }
    onPositionChange(data) {
        this.setState(data);
    }
    onLoadFileError(error) {
        if (typeof this.props.onLoadFileError === 'function') this.props.onLoadFileError(error);
    }
    onLoadFile(file) {
        if (typeof this.props.onLoadFile === 'function') this.props.onLoadFile(file);
        this.setState({ isContainsFile: true });
    }

    onSaveButtonClick() {
        this.state.isContainsFile ?
            this.props.onSave(this.state.isContainsFile, {
                x: this.state.x,
                y: this.state.y,
                width: this.state.width,
                height: this.state.height
            }, this.state.croppedImage) :

            this.props.onSave(this.state.isContainsFile);
    }

    onClose() {
        this.setState({ visible: false });
        this.props.onClose();
    }
    popstate() {
        window.history.go(1);
        this.onClose();
        window.removeEventListener('popstate', this.popstate, false);
    }
    componentDidUpdate(prevProps) {
        if (this.props.visible !== prevProps.visible) {
            this.setState({ visible: this.props.visible });
        }
        if (this.props.displayType !== prevProps.displayType) {
            this.setState({ displayType: this.props.displayType !== 'auto' ? this.props.displayType : window.innerWidth < desktop.match(/\d+/)[0] ? 'aside' : 'modal' });
        }
        if (this.state.visible && this.state.displayType === "aside") {
            window.addEventListener("popstate", this.popstate, false);
        }
    }
    componentDidMount() {
        window.addEventListener('resize', this.throttledResize);
    }
    componentWillUnmount() {
        window.removeEventListener('resize', this.throttledResize);
    }
    render() {

        return (
            this.state.displayType === "modal" ?
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
                            size="medium"
                            onClick={this.onSaveButtonClick}
                        />
                    ]}
                    onClose={this.props.onClose}
                />
                :
                <SidePanel
                    visible={this.state.visible}
                    scale={true}
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
                            size="medium"
                            onClick={this.onSaveButtonClick}
                        />
                    ]}
                    onClose={this.onClose}
                />
        );
    }
}

AvatarEditor.propTypes = {
    visible: PropTypes.bool,
    headerLabel: PropTypes.string,
    chooseFileLabel: PropTypes.string,
    saveButtonLabel: PropTypes.string,
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
    displayType: PropTypes.oneOf(['auto', 'modal', 'aside']),

};

AvatarEditor.defaultProps = {
    visible: false,
    maxSize: 1, //1MB
    headerLabel: 'Edit Photo',
    saveButtonLabel: 'Save',
    accept: ['image/png', 'image/jpeg'],
    displayType: 'auto'
};

export default AvatarEditor;


