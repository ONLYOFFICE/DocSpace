import React from "react";
import PropTypes from "prop-types";

import Viewer from 'react-viewer';
import { Icons } from "asc-web-components";
import styled from "styled-components";

const StyledViewer = styled(Viewer)`

    .react-viewer-attribute{
        display: none;
    }
    .react-viewer-toolbar li{
        width: 40px;
        border-radius: 2px;
        cursor: pointer;
        line-height: 24px;
        
    }

    .react-viewer-btn{
        background-color: none;
        &:hover{
            background-color: rgba(200, 200, 200, 0.2);
        }
    }
    li[data-key='prev']{
        left: 20px
    }
    li[data-key='next']{
        right: 20px
    }
    li[data-key='prev'],
    li[data-key='next']{
        position: fixed;
        top: calc(50% - 20px);
        
        height: auto;
        background: none;

        &:hover{
            background: none;
        }
    }


`

const NextButton = styled.div`
   
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



const MediaScrollButton = props => {
  //console.log("Backdrop render");
  return (
    <NextButton {...props} />
  );
}

var customToolbar = [
    {
        key: 'zoomIn',
        actionType: 1,
        render:  <Icons.PlusIcon size="medium" isfill={true} color="#fff" />
    },
    {
        key: 'zoomOut',
        actionType: 2,
        render:  <Icons.CrossIcon size="medium" isfill={true} color="#fff"/>
    },
    {
        key: 'reset',
        actionType: 7,
        render:  <Icons.AccessFormIcon size="medium" isfill={true} color="#fff"/>
    },
    {
        key: 'rotateLeft',
        actionType: 5,
        render:  <Icons.RotateIcon size="medium" isfill={true} color="#fff"/>
    },
    {
        key: 'rotateRight',
        actionType: 6,
        render:  <Icons.RotateIcon size="medium" isfill={true} color="#fff"/>
    },
    {
        key: 'prev',
        actionType: 3,
        render:  <MediaScrollButton orientation = "right" />
    },
    {
        key: 'next',
        actionType: 4,
        render:  <MediaScrollButton orientation = "left"/>
        
    },
];

class ImageViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: false,
          };
    }

    render(){

        const { className, visible, onClose, images } = this.props;

        return(
            <div> 
                <StyledViewer
                    visible={visible}
                    onClose={onClose}
                    customToolbar={(toolbars) => {
                        return customToolbar;
                      }}
                    images={images}
                />
            </div>
        )
    };
}

ImageViewer.propTypes = {}

ImageViewer.defaultProps = {}

export default ImageViewer;