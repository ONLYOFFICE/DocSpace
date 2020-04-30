import React from "react";
import PropTypes from "prop-types";

import Viewer from 'react-viewer';
import { Icons } from "asc-web-components";
import styled from "styled-components";

const StyledViewer = styled(Viewer)`

    .react-viewer-footer{
        bottom: 5px!important;
        z-index: 4001!important;
    }
    .react-viewer-canvas{
        z-index: 4000!important;
    }
    .react-viewer-navbar,
    .react-viewer-mask{
        display: none
    }
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
        background-color: transparent;
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


var customToolbar = [
    {
        key: 'zoomIn',
        actionType: 1,
        render: <Icons.PlusIcon size="medium" isfill={true} color="#fff" />
    },
    {
        key: 'zoomOut',
        actionType: 2,
        render: <Icons.CrossIcon size="medium" isfill={true} color="#fff" />
    },
    {
        key: 'reset',
        actionType: 7,
        render: <Icons.AccessFormIcon size="medium" isfill={true} color="#fff" />
    },
    {
        key: 'rotateLeft',
        actionType: 5,
        render: <Icons.RotateIcon size="medium" isfill={true} color="#fff" />
    },
    {
        key: 'rotateRight',
        actionType: 6,
        render: <Icons.RotateIcon size="medium" isfill={true} color="#fff" />
    }
];

class ImageViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: false,
        };
    }

    render() {

        const { className, visible, images } = this.props;

        return (
            <div className={className}>
                <StyledViewer
                    visible={visible}
                    customToolbar={(toolbars) => {
                        return customToolbar;
                    }}
                    images={images}
                />
            </div>
        )
    };
}

ImageViewer.propTypes = {
    className: PropTypes.string,
    visible: PropTypes.bool,
    images: PropTypes.arrayOf(PropTypes.object)
}

export default ImageViewer;