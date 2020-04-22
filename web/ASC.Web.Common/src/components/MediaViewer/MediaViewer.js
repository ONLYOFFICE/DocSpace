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
   
    cursore: pointer;
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

class MediaViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: true,
          };
    }

    render(){
        return(
            <StyledMediaViewer>
               
               <div className = "videoViewerOverlay"></div>
               <MediaScrollButton orientation = "right" />
               <MediaScrollButton orientation = "left" />
                <div>
                    <div className = "details">
                        <div className = "title">123.123</div>
                        <VideoControlBtn  onClick={this.props.onClick} className = "mediaPlayerClose">
                            <Icons.CrossIcon size="medium" isfill={true} color="#fff" />
                        </VideoControlBtn>
                    </div>
                </div>
                <StyledVideoViewer />
                <div className = "mediaViewerToolbox"></div>
                <span>
                    <VideoControlBtn>
                        <Icons.CatalogTrashIcon size="medium" isfill={true} color="#fff" />
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