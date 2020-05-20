import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons } from "asc-web-components";

import ImageViewer from "./sub-components/image-viewer"
import VideoViewer from "./sub-components/video-viewer"
import MediaScrollButton from "./sub-components/scroll-button"
import ControlBtn from "./sub-components/control-btn"
import StyledMediaViewer from "./StyledMediaViewer"
import isEqual from "lodash/isEqual";

const StyledVideoViewer = styled(VideoViewer)`
    z-index: 4001;
`
const mediaTypes = Object.freeze({
    audio: 1,
    video: 2
});
class MediaViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: this.props.visible,
            allowConvert: true,
            playlist: this.props.playlist,
            playlistPos: 0,
        };

        this.detailsContainer = React.createRef();
        this.viewerToolbox = React.createRef();
    }

    componentDidUpdate(prevProps) {
        if (this.props.visible !== prevProps.visible) {
            this.setState(
                {
                    visible: this.props.visible,
                    playlistPos: this.props.playlist.length > 0 ? this.props.playlist.find(file => file.fileId === this.props.currentFileId).id : 0
                }
            );
        }
        if (this.props.visible && this.props.visible === prevProps.visible && !isEqual(this.props.playlist, prevProps.playlist)) {
            let playlistPos = 0;
            if (this.props.playlist.length > 0) {
                if (this.props.playlist.length - 1 < this.state.playlistPos) {
                    playlistPos = this.props.playlist.length - 1;
                }
                this.setState(
                    {
                        playlist: this.props.playlist,
                        playlistPos: playlistPos
                    }
                );
            } else {
                this.props.onEmptyPlaylistError();
                this.setState(
                    {
                        visible: false
                    }
                );
            }
        }

    }
    mapSupplied = {
        ".aac": { supply: "m4a", type: mediaTypes.audio },
        ".flac": { supply: "mp3", type: mediaTypes.audio },
        ".m4a": { supply: "m4a", type: mediaTypes.audio },
        ".mp3": { supply: "mp3", type: mediaTypes.audio },
        ".oga": { supply: "oga", type: mediaTypes.audio },
        ".ogg": { supply: "oga", type: mediaTypes.audio },
        ".wav": { supply: "wav", type: mediaTypes.audio },

        ".f4v": { supply: "m4v", type: mediaTypes.video },
        ".m4v": { supply: "m4v", type: mediaTypes.video },
        ".mov": { supply: "m4v", type: mediaTypes.video },
        ".mp4": { supply: "m4v", type: mediaTypes.video },
        ".ogv": { supply: "ogv", type: mediaTypes.video },
        ".webm": { supply: "webmv", type: mediaTypes.video },
        ".wmv": { supply: "m4v", type: mediaTypes.video, convertable: true },
        ".avi": { supply: "m4v", type: mediaTypes.video, convertable: true },
        ".mpeg": { supply: "m4v", type: mediaTypes.video, convertable: true },
        ".mpg": { supply: "m4v", type: mediaTypes.video, convertable: true }
    };

    canImageView = function (ext) {
        return this.props.extsImagePreviewed.indexOf(ext) != -1;
    };
    canPlay = (fileTitle, allowConvert) => {

        var ext = fileTitle[0] === "." ? fileTitle : this.getFileExtension(fileTitle);

        var supply = this.mapSupplied[ext];

        var canConv = allowConvert || this.props.allowConvert;

        return !!supply && this.props.extsMediaPreviewed.indexOf(ext) != -1
            && (!supply.convertable || canConv);
    };

    getFileExtension = (fileTitle) => {
        if (!fileTitle) {
            return "";
        }
        fileTitle = fileTitle.trim();
        var posExt = fileTitle.lastIndexOf(".");
        return 0 <= posExt ? fileTitle.substring(posExt).trim().toLowerCase() : "";
    };

    prevMedia = () => {

        let currentPlaylistPos = this.state.playlistPos;
        currentPlaylistPos--;
        if (currentPlaylistPos < 0)
            currentPlaylistPos = this.state.playlist.length - 1;

        this.setState({
            playlistPos: currentPlaylistPos
        });

    };

    nextMedia = () => {

        let currentPlaylistPos = this.state.playlistPos;
        currentPlaylistPos = (currentPlaylistPos + 1) % this.state.playlist.length;

        this.setState({
            playlistPos: currentPlaylistPos
        });
    };
    getOffset = () => {
        if (this.detailsContainer.current && this.viewerToolbox.current) {
            return this.detailsContainer.current.offsetHeight + this.viewerToolbox.current.offsetHeight;
        } else {
            return 0;
        }
    }
    onDelete = () => {
        let currentFileId = this.state.playlist.length > 0 ? this.state.playlist.find(file => file.id === this.state.playlistPos).fileId : 0;
        this.props.onDelete && this.props.onDelete(currentFileId);
    }
    onDownload = () => {
        let currentFileId = this.state.playlist.length > 0 ? this.state.playlist.find(file => file.id === this.state.playlistPos).fileId : 0;
        this.props.onDownload && this.props.onDownload(currentFileId);
    }
    render() {

        let currentPlaylistPos = this.state.playlistPos;
        let currentFileId = this.state.playlist.length > 0 ? this.state.playlist.find(file => file.id === currentPlaylistPos).fileId : 0;

        let currentFile = this.state.playlist[currentPlaylistPos];
        let fileTitle = currentFile.title;
        let url = currentFile.src;

        let isImage = false;
        let isVideo = false;
        let canOpen = true;

        var ext = this.getFileExtension(fileTitle) ? this.getFileExtension(fileTitle) : this.getFileExtension(url);

        if (!this.canPlay(ext) && !this.canImageView(ext)) {
            canOpen = false;
            this.props.onError && this.props.onError();
        }

        if (this.canImageView(ext)) {
            isImage = true;
        } else {
            isImage = false;
            isVideo = this.mapSupplied[ext] ? this.mapSupplied[ext].type == mediaTypes.video : false;
        }

        if (this.mapSupplied[ext])
            if (!isImage && this.mapSupplied[ext].convertable && !url.includes("#")) {
                url += (url.includes("?") ? "&" : "?") + "convpreview=true";
            }

        return (
            <StyledMediaViewer visible={this.state.visible}>

                <div className="videoViewerOverlay"></div>
                <MediaScrollButton orientation="right" onClick={this.prevMedia} />
                <MediaScrollButton orientation="left" onClick={this.nextMedia} />
                <div>
                    <div className="details" ref={this.detailsContainer}>
                        <div className="title">{fileTitle}</div>
                        <ControlBtn onClick={this.props.onClose && (this.props.onClose)} className="mediaPlayerClose">
                            <Icons.CrossIcon size="medium" isfill={true} color="#fff" />
                        </ControlBtn>
                    </div>
                </div>
                {canOpen &&
                    (
                        isImage ?
                            <ImageViewer
                                visible={this.state.visible}
                                onClose={() => { this.setState({ visible: false }); }}
                                images={[
                                    { src: url, alt: '' }
                                ]}
                            />
                            :
                            <StyledVideoViewer url={url} playing={this.state.visible} isVideo={isVideo} getOffset={this.getOffset} />
                    )
                }
                <div className="mediaViewerToolbox" ref={this.viewerToolbox}>
                    <span>
                        {
                            this.props.canDelete(currentFileId) &&
                            <ControlBtn onClick={this.onDelete}>
                                <div className="deleteBtnContainer">
                                    <Icons.MediaDeleteIcon size="scale" />
                                </div>
                            </ControlBtn>
                        }
                        {
                            this.props.canDownload(currentFileId) &&
                            <ControlBtn onClick={this.onDownload}>
                                <div className="downloadBtnContainer">
                                    <Icons.MediaDownloadIcon size="scale" />
                                </div>
                            </ControlBtn>
                        }
                    </span>
                </div>

            </StyledMediaViewer>
        )
    }
}

MediaViewer.propTypes = {
    allowConvert: PropTypes.bool,
    visible: PropTypes.bool,
    currentFileId: PropTypes.number,
    playlist: PropTypes.arrayOf(PropTypes.object),
    extsImagePreviewed: PropTypes.arrayOf(PropTypes.string),
    extsMediaPreviewed: PropTypes.arrayOf(PropTypes.string),
    onError: PropTypes.func,
    canDelete: PropTypes.func,
    canDownload: PropTypes.func,
    onDelete: PropTypes.func,
    onDownload: PropTypes.func,
    onClose: PropTypes.func,
    onEmptyPlaylistError: PropTypes.func
}

MediaViewer.defaultProps = {
    currentFileId: 0,
    visible: false,
    allowConvert: true,
    canDelete: () => { return true },
    canDownload: () => { return true }
}

export default MediaViewer;