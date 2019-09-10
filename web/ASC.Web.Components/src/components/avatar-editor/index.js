import React, { memo } from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import ModalDialog from '../modal-dialog'
import Button from '../button'
import { Text } from '../text'
import Avatar from 'react-avatar-edit'
import { default as ASCAvatar } from '../avatar/index'

const StyledASCAvatar = styled(ASCAvatar)`
    display: inline-block;
    vertical-align: bottom;
`;
const StyledAvatarContainer = styled.div`
    text-align: center;
    div:first-child {
        margin: 0 auto;
    }
`;
class AvatarEditorBody extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            croppedImage: null,
            src: this.props.image,
            hasMaxSizeError: false
        }
        this.onCrop = this.onCrop.bind(this)
        this.onClose = this.onClose.bind(this)
        this.onBeforeFileLoad = this.onBeforeFileLoad.bind(this)
        this.onFileLoad = this.onFileLoad.bind(this)

    }
    onClose() {
        this.props.onCloseEditor();
        this.setState({ croppedImage: null })
    }
    onCrop(croppedImage) {
        this.props.onCropImage(croppedImage);
        this.setState({ croppedImage })
    }
    onBeforeFileLoad(elem) {
        if (elem.target.files[0].size > this.props.maxSize * 1000000) {
            this.setState({
                hasMaxSizeError: true
            });
            elem.target.value = "";
        }else if(this.state.hasMaxSizeError){
            this.setState({
                hasMaxSizeError: false
            });
        }
    }
    onFileLoad(file){
        let reader = new FileReader();
        let _this = this;
        reader.onloadend = () => {
            _this.props.onFileLoad(reader.result);
        };
        reader.readAsDataURL(file)
    }
    render() {
        return (
            <StyledAvatarContainer>
                <Avatar
                    width={400}
                    height={295}
                    imageWidth={400}
                    cropRadius={50}
                    onCrop={this.onCrop}
                    onClose={this.onClose}
                    onBeforeFileLoad={this.onBeforeFileLoad}
                    onFileLoad={this.onFileLoad}
                    label={this.props.label}
                    src={this.state.src}
                />
                {this.state.croppedImage && (
                    <div>
                        <StyledASCAvatar
                            size='max'
                            role='user'
                            source={this.state.croppedImage}
                            editing={false}
                        />
                        <StyledASCAvatar
                            size='big'
                            role='user'
                            source={this.state.croppedImage}
                            editing={false}
                        />
                    </div>
                    )
                }
                {
                    this.state.hasMaxSizeError &&
                    <Text.Body as='span' color="#ED7309" isBold={true}>
                        {this.props.maxSizeErrorLabel}
                    </Text.Body>
                }
            </StyledAvatarContainer>
        );
    }
}

class AvatarEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            defaultImage: null,
            croppedImage: null,
            visible: props.value
        };

        this.onClose = this.onClose.bind(this);

        this.onCropImage = this.onCropImage.bind(this);
        this.onCloseEditor = this.onCloseEditor.bind(this);

        this.onFileLoad = this.onFileLoad.bind(this);
        this.onSaveButtonClick = this.onSaveButtonClick.bind(this);

    }
    onFileLoad(file){
        this.setState({ defaultImage: file });
    }
    onSaveButtonClick() {
        this.props.onSave({
            defaultImage: this.state.defaultImage,
            croppedImage: this.state.croppedImage
        });
        this.setState({ visible: false });
    }
    onCloseEditor() {
        this.setState({
            croppedImage: null
        });
    }
    onCropImage(result) {
        this.setState({
            croppedImage: result
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
                        maxSize={this.props.maxSize}
                        image={this.props.image}
                        onCropImage={this.onCropImage}
                        onCloseEditor={this.onCloseEditor}
                        label={this.props.chooseFileLabel}
                        maxSizeErrorLabel={this.props.maxSizeErrorLabel}
                        onFileLoad={this.onFileLoad}
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

    onSave: PropTypes.func,
    onClose: PropTypes.func
};

AvatarEditor.defaultProps = {
    visible: false,
    maxSize: 1, //1MB
    chooseFileLabel: 'Choose a file',
    headerLabel: 'Edit Photo',
    saveButtonLabel: 'Save',
    cancelButtonLabel: 'Cancel',
    maxSizeErrorLabel: 'File is too big'
};

export default AvatarEditor;


