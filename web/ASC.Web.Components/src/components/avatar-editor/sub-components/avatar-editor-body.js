import React from 'react'
import styled from 'styled-components'
import Dropzone from 'react-dropzone'
import ReactAvatarEditor from 'react-avatar-editor'
import PropTypes from 'prop-types'
import { default as ASCAvatar } from '../../avatar/index'
import accepts from 'attr-accept'
import Text from '../../text'
import { tablet } from '../../../utils/device';

const StyledErrorContainer = styled.div`
    p{
        text-align: center
    }
`;

const CloseButton = styled.a`
    cursor: pointer;
    position: absolute;
    right: 33px;
    top: 4px;
    width: 16px;
    height: 16px;

    &:before, &:after {
        position: absolute;
        left: 8px;
        content: ' ';
        height: 16px;
        width: 1px;
        background-color: #D8D8D8;
    }
    &:before {
        transform: rotate(45deg);
    }
    &:after {
        transform: rotate(-45deg);
    }
    @media ${tablet} {
        right: calc(50% - 147px);
    }
`;

const DropZoneContainer = styled.div`
    box-sizing: border-box;
    display: block;
    width: 100%;
    height: 300px;
    border: 1px dashed #ccc;
    text-align: center;
    padding: 10em 0;
    margin: 0 auto;
    p{
        margin: 0;
        cursor: default
    }
    .desktop {
        display: block;
    };
    .mobile {
        display: none;
    };
    @media ${tablet} {
        .desktop {
            display: none;
        };
        .mobile {
            display: block;
        };
    }
`;
const StyledAvatarContainer = styled.div`
    text-align: center;

    .custom-range{
        width: 100%;
        display: block
    }
    .avatar-container{
        display: inline-block;
        vertical-align: top;
    }
    .editor-container{
        display: inline-block;
        width: auto;
        padding: 0 30px;
        position: relative;
        @media ${tablet} {
            padding: 0;
        }
    }
`;
const StyledASCAvatar = styled(ASCAvatar)`
    display: block;
    @media ${tablet} {
        display: none
    }
`;

class AvatarEditorBody extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            image: this.props.image ? this.props.image : "",
            scale: 1,
            croppedImage: '',

            errorText: null
        }

        this.setEditorRef = React.createRef();

        this.handleScale = this.handleScale.bind(this);
        this.onWheel = this.onWheel.bind(this);

        this.onTouchStart = this.onTouchStart.bind(this);
        this.onTouchMove = this.onTouchMove.bind(this);
        this.onTouchEnd = this.onTouchEnd.bind(this);

        this.distance = this.distance.bind(this);

        this.onImageChange = this.onImageChange.bind(this);
        this.onImageReady = this.onImageReady.bind(this);
        this.deleteImage = this.deleteImage.bind(this);

        this.onDropAccepted = this.onDropAccepted.bind(this);
        this.onDropRejected = this.onDropRejected.bind(this);


        this.onPositionChange = this.onPositionChange.bind(this);

    }

    onPositionChange(position) {
        this.props.onPositionChange({
            x: position.x,
            y: position.y,
            width: this.setEditorRef.current.getImage().width,
            height: this.setEditorRef.current.getImage().height
        });
    }
    onDropRejected(rejectedFiles) {
        if (!accepts(rejectedFiles[0], this.props.accept)) {
            this.props.onLoadFileError(0);
            this.setState({
                errorText: this.props.unknownTypeError
            })
            return;
        } else if (rejectedFiles[0].size > this.props.maxSize) {
            this.props.onLoadFileError(1);
            this.setState({
                errorText: this.props.maxSizeFileError
            })
            return;
        }
        this.setState({
            errorText: this.props.unknownError
        })
        this.props.onLoadFileError(2);
    }
    onDropAccepted(acceptedFiles) {
        this.setState({
            image: acceptedFiles[0],
            errorText: null
        });
        this.props.onLoadFile(acceptedFiles[0]);
    }
    deleteImage() {
        this.setState({
            image: '',
            croppedImage: ''
        });
        this.props.deleteImage();
    }
    onImageChange() {
        if(this.setEditorRef.current !== null){
            this.setState({
                croppedImage: this.setEditorRef.current.getImage().toDataURL()
            });
            this.props.onImageChange(this.setEditorRef.current.getImage().toDataURL());
        }
    }
    dist = 0
    scaling = false
    curr_scale = 1.0
    scale_factor = 1.0
    distance(p1, p2) {
        return (Math.sqrt(Math.pow((p1.clientX - p2.clientX), 2) + Math.pow((p1.clientY - p2.clientY), 2)));
    }
    onTouchStart(evt) {
        evt.preventDefault();
        var tt = evt.targetTouches;
        if (tt.length >= 2) {
            this.dist = this.distance(tt[0], tt[1]);
            this.scaling = true;
        } else {
            this.scaling = false;
        }
    }
    onTouchMove(evt) {
        evt.preventDefault();
        var tt = evt.targetTouches;
        if (this.scaling) {
            this.curr_scale = this.distance(tt[0], tt[1]) / this.dist * this.scale_factor;
            let scale = (Math.round(this.curr_scale) * 10) / 10;
            this.setState({
                scale: scale < 1 ? 1 : scale > 5 ? 5 : scale
            });
            this.props.onSizeChange({
                width: this.setEditorRef.current.getImage().width,
                height: this.setEditorRef.current.getImage().height
            });
        }
    }
    onTouchEnd(evt) {
        var tt = evt.targetTouches;
        if (tt.length < 2) {
            this.scaling = false;
            if (this.curr_scale < 1) {
                this.scale_factor = 1;
            } else {
                if (this.curr_scale > 5) {
                    this.scale_factor = 5;
                } else {
                    this.scale_factor = this.curr_scale;
                }
            }
        } else {
            this.scaling = true;
        }
    }
    onWheel(e) {
        e = e || window.event;
        const delta = e.deltaY || e.detail || e.wheelDelta;
        let scale = delta > 0 && this.state.scale === 1 ? 1 : this.state.scale - (delta / 100) * 0.1;
        scale = Math.round(scale * 10) / 10;
        this.setState({
            scale: scale < 1 ? 1 : scale > 5 ? 5 : scale
        });
        this.props.onSizeChange({
            width: this.setEditorRef.current.getImage().width,
            height: this.setEditorRef.current.getImage().height
        });
    }

    handleScale = e => {
        const scale = parseFloat(e.target.value);
        this.setState({ scale });
        this.props.onSizeChange({
            width: this.setEditorRef.current.getImage().width,
            height: this.setEditorRef.current.getImage().height
        });
    };
    onImageReady() {
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
    }
    componentDidUpdate(prevProps){
        if(prevProps.image !== this.props.image){
            this.setState({
                image: this.props.image
            });
        }
    }
    render() {
        return (
            <div
                onWheel={this.onWheel}
                onTouchStart={this.onTouchStart}
                onTouchMove={this.onTouchMove}
                onTouchEnd={this.onTouchEnd}
            >
                {this.state.image === '' ?
                    <Dropzone
                        onDropAccepted={this.onDropAccepted}
                        onDropRejected={this.onDropRejected}
                        maxSize={this.props.maxSize}
                        accept={this.props.accept}>
                        {({ getRootProps, getInputProps }) => (
                            <DropZoneContainer
                                {...getRootProps()}
                            >
                                <input {...getInputProps()} />
                                <p className="desktop">{this.props.chooseFileLabel}</p>
                                <p className="mobile">{this.props.chooseMobileFileLabel}</p>
                            </DropZoneContainer>
                        )}
                    </Dropzone> :
                    <StyledAvatarContainer>
                        <div className='editor-container'>
                            <ReactAvatarEditor
                                ref={this.setEditorRef}
                                width={250}
                                borderRadius={200}
                                scale={this.state.scale}
                                height={250}
                                className="react-avatar-editor"
                                image={this.state.image}
                                color={[0, 0, 0, .5]}
                                onImageChange={this.onImageChange}
                                onPositionChange={this.onPositionChange}
                                onImageReady={this.onImageReady}
                            />
                            <input
                                id='scale'
                                type='range'
                                className='custom-range'
                                onChange={this.handleScale}
                                min={this.state.allowZoomOut ? '0.1' : '1'}
                                max='5'
                                step='0.01'
                                value={this.state.scale}
                            />
                            <CloseButton onClick={this.deleteImage}></CloseButton>
                        </div>
                        <div className='avatar-container'>
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

                    </StyledAvatarContainer>
                }
                <StyledErrorContainer key="errorMsg">
                    {this.state.errorText !== null &&
                        <Text
                            as="p"
                            color="#C96C27"
                            isBold={true}
                        >{this.state.errorText}</Text>
                    }
                </StyledErrorContainer>
            </div>
        );
    }
}

AvatarEditorBody.propTypes = {
    onImageChange: PropTypes.func,
    onPositionChange: PropTypes.func,
    onSizeChange: PropTypes.func,
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
    unknownError: PropTypes.string

};

AvatarEditorBody.defaultProps = {
    accept: ['image/png', 'image/jpeg'],
    maxSize: Number.MAX_SAFE_INTEGER,
    chooseFileLabel: "Drop files here, or click to select files",
    chooseMobileFileLabel: "Click to select files",
    unknownTypeError: "Unknown image file type",
    maxSizeFileError: "Maximum file size exceeded",
    unknownError: "Error"
};
export default AvatarEditorBody;