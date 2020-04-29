import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons } from "asc-web-components";

import ImageViewer from "./sub-components/image-viewer"
import VideoViewer from "./sub-components/video-viewer"
import MediaScrollButton from "./sub-components/scroll-button"
import ControlBtn from "./sub-components/control-btn"

const StyledVideoViewer = styled(VideoViewer)`
    z-index: 4001;
`
const StyledMediaViewer = styled.div`
    
    color: #d1d1d1;
    display: ${props => props.visible ? "block" : "none"};

    .videoViewerOverlay{
        position: fixed;
        z-index: 4000;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: black;
        opacity: 0.5;
    }
    .mediaViewerToolbox{
        z-index: 4001;
        padding-top: 14px;
        padding-bottom: 14px;
        height: 20px;
        width: 100%;
        background-color: rgba(11,11,11,0.7);
        position: fixed;
        bottom: 0;
        left: 0;
        text-align: center;

      
    }
    span{
        position: absolute;
        right: 0;
        bottom: 5px;
        margin-right: 10px;
        z-index: 4005;
    }
    .details{
        z-index: 4001;
        font-size: 14px;
        font-weight: bold;
        text-align: center;
        white-space: nowrap;
        padding-top: 14px;
        padding-bottom: 14px;
        height: 20px;
        width: 100%;
        background: rgba(17,17,17,0.867);
        position: fixed;
        top: 0;
        left: 0;
    }

    .mediaPlayerClose{
        position: fixed;
        top: 4px;
        right: 10px;
        height: 30px;
    }
  
`;

var audio = 1;
var video = 2;

class MediaViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: this.props.visible,
            allowConvert: true,
            playlist: this.props.playlist,
            playlistPos: 0,
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.visible !== prevProps.visible) {
          this.setState(
            {
                visible: this.props.visible
            }
          );
        }
      }
    mapSupplied = {
        ".aac": { supply: "m4a", type: audio },
        ".flac": { supply: "mp3", type: audio },
        ".m4a": { supply: "m4a", type: audio },
        ".mp3": { supply: "mp3", type: audio },
        ".oga": { supply: "oga", type: audio },
        ".ogg": { supply: "oga", type: audio },
        ".wav": { supply: "wav", type: audio },

        ".f4v": { supply: "m4v", type: video },
        ".m4v": { supply: "m4v", type: video },
        ".mov": { supply: "m4v", type: video },
        ".mp4": { supply: "m4v", type: video },
        ".ogv": { supply: "ogv", type: video },
        ".webm": { supply: "webmv", type: video },
        ".wmv": { supply: "m4v", type: video, convertable: true },
        ".avi": { supply: "m4v", type: video, convertable: true },
        ".mpeg": { supply: "m4v", type: video, convertable: true },
        ".mpg": { supply: "m4v", type: video, convertable: true }
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
        if (typeof fileTitle == "undefined" || fileTitle == null) {
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

    render() {

        let currentPlaylistPos = this.state.playlistPos;
        let fileTitle = this.state.playlist[currentPlaylistPos].title;
        let url = this.state.playlist[currentPlaylistPos].src;
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
            isVideo = this.mapSupplied[ext] ? this.mapSupplied[ext].type == video : false;
        }
        
        return (
            <StyledMediaViewer visible={this.state.visible}>

                <div className="videoViewerOverlay"></div>
                <MediaScrollButton orientation="right" onClick={this.prevMedia} />
                <MediaScrollButton orientation="left" onClick={this.nextMedia} />
                <div>
                    <div className="details">
                        <div className="title">{fileTitle}</div>
                        <ControlBtn onClick={this.props.onClose && (() => {this.props.onClose()})} className="mediaPlayerClose">
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
                            <StyledVideoViewer url={url} isVideo={isVideo} />
                    )
                }
                <div className="mediaViewerToolbox"></div>
                <span>
                    <ControlBtn onClick={this.props.onDelete && (() => {this.props.onDelete(this.state.playlistPos)})}>
                        <Icons.CatalogTrashIcon size="medium" isfill={true} color="#fff" />
                    </ControlBtn>

                    <ControlBtn onClick={this.props.onDownload && (() => {this.props.onDownload(this.state.playlistPos)})}>
                        <Icons.DownloadIcon size="medium" isfill={true} color="#fff" />
                    </ControlBtn>
                </span>
            </StyledMediaViewer>
        )
    }
}

MediaViewer.propTypes = {
    allowConvert: PropTypes.bool,
    visible: PropTypes.bool,
    playlist: PropTypes.arrayOf(PropTypes.object),
    extsImagePreviewed: PropTypes.arrayOf(PropTypes.string),
    extsMediaPreviewed: PropTypes.arrayOf(PropTypes.string),
    onError: PropTypes.func,
    canDelete: PropTypes.func,
    onDelete: PropTypes.func,
    onDownload: PropTypes.func,
    onClose: PropTypes.func
}

MediaViewer.defaultProps = {
    allowConvert: true,
    canDelete: () => { return true }
}

export default MediaViewer;