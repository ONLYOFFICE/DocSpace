import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons } from "asc-web-components";

import ImageViewer from "./sub-components/image-viewer"
import VideoViewer from "./sub-components/video-viewer"

const StyledVideoViewer = styled(VideoViewer)`
    z-index: 4001;
`
const StyledMediaViewer = styled.div`
    
    color: #d1d1d1;
   
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
const ScrollButton = styled.div`
   
    cursor: pointer;
    z-index: 4001;
    position: fixed;
    top: calc(50% - 20px);
   
    background: none;

    &:hover{
        background: none;
    }
    ${props => props.orientation != "left" ? 'left: 20px;' : 'right: 20px;'}
    
    width: 40px;
    height: 40px;
    background-color: rgba(11, 11, 11, 0.7);
    border-radius: 50%;

    &:hover{
        background-color: rgba(200, 200, 200, 0.2);
    }

    &:before{
        content:'';
        top: 12px;
        left: ${props => props.orientation == "left" ? '9px;' : '15px;'};
        position: absolute;
        border: solid #fff;
        border-width: 0 2px 2px 0;
        display: inline-block;
        padding: 7px;
        transform: ${props => props.orientation == "left" ? 'rotate(-45deg)' : 'rotate(135deg)'};
        -webkit-transform: ${props => props.orientation == "left" ? 'rotate(-45deg)' : 'rotate(135deg)'};
    }
  
`;

const StyledVideoControlBtn = styled.div`
    display: inline-block;
    height: 30px;
    line-height: 25px;
    margin: 5px;
    width: 40px;
    border-radius: 2px;
    cursor: pointer;
    text-align: center;

    &:hover{
        background-color: rgba(200,200,200,0.2);
    }
`;

const VideoControlBtn = props => {
    return (
        <StyledVideoControlBtn {...props} >
            {props.children}
        </StyledVideoControlBtn>
    );
}


const MediaScrollButton = props => {
  return (
    <ScrollButton {...props} />
  );
}
var audio = 1;
var video = 2;

class MediaViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: true,
            allowConvert: true,
            playlist: this.props.playlist,
            playlistPos: 0,
        };
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

        return !!supply &&  this.props.extsMediaPreviewed.indexOf(ext) != -1
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

    render(){
        let currentPlaylistPos = this.state.playlistPos;
        let fileTitle = this.state.playlist[currentPlaylistPos].title;
        let url = this.state.playlist[currentPlaylistPos].src;
        let isImage = false;
        var isVideo = false;

        var ext = this.getFileExtension(fileTitle) ? this.getFileExtension(fileTitle) : this.getFileExtension(url);

        if (!this.canPlay(ext) && !this.canImageView(ext)) {
            console.log("ERROR")
        }

        if (this.canImageView(ext)) {
            isImage = true;
        } else {
            isImage = false; 
            isVideo = this.mapSupplied[ext] ? this.mapSupplied[ext].type == video : false;
        }

        return(
            <StyledMediaViewer>
               
               <div className = "videoViewerOverlay"></div>
               <MediaScrollButton orientation = "right" onClick={this.prevMedia}/>
               <MediaScrollButton orientation = "left" onClick={this.nextMedia}/>
                <div>
                    <div className = "details">
                        <div className = "title">{fileTitle}</div>
                        <VideoControlBtn  onClick={this.props.onClick} className = "mediaPlayerClose">
                            <Icons.CrossIcon size="medium" isfill={true} color="#fff" />
                        </VideoControlBtn>
                    </div>
                </div>
                {isImage ?
                    <ImageViewer 
                        visible={this.state.visible}
                        onClose={() => { this.setState({ visible: false }); } }
                        images={[
                            {src: url, alt: ''}
                        ]}
                    /> 
                    :
                    <StyledVideoViewer url = {url} isVideo={isVideo}/>
                }
                <div className = "mediaViewerToolbox"></div>
                <span>
                    <VideoControlBtn> 
                        <Icons.CatalogTrashIcon  size="medium" isfill={true} color="#fff" />
                    </VideoControlBtn>

                    <VideoControlBtn>
                        <Icons.DownloadIcon size="medium" isfill={true} color="#fff" />
                    </VideoControlBtn>
                </span>
               { /*  <StyledVideoViewer />
              
              <ImageViewer 
                    visible={this.state.visible}
                    onClose={() => { this.setState({ visible: false }); } }
                    images={[
                        {src: 'http://localhost/Products/Files/httphandlers/filehandler.ashx?action=download&fileid=2025993', alt: ''},
                        {src: 'http://localhost/Products/Files/httphandlers/filehandler.ashx?action=download&fileid=2025992', alt: ''}
                        
                    ]}
                /> 
               */} 
            </StyledMediaViewer>
        )
    };
}

MediaViewer.propTypes = {}

MediaViewer.defaultProps = {}

export default MediaViewer;