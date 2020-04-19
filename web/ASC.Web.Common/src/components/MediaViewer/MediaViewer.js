import React from "react";
import PropTypes from "prop-types";

import ImageViewer from "./sub-components/image-viewer"
import VideoViewer from "./sub-components/video-viewer"

class MediaViewer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            visible: false,
          };
    }

    render(){
        return(
            <div>
                <button onClick={() => { this.setState({ visible: !this.state.visible }); } }>show</button>

                <VideoViewer />
               {/*
               <ImageViewer 
                    visible={this.state.visible}
                    onClose={() => { this.setState({ visible: false }); } }
                    images={[
                        {src: '', alt: ''},
                        {src: '', alt: ''}
                        
                    ]}
                /> 
               */} 
            </div>
        )
    };
}

MediaViewer.propTypes = {}

MediaViewer.defaultProps = {}

export default MediaViewer;