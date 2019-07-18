import React from "react";
import IconButton from '../icon-button';

class CloseButton extends React.Component {
    constructor(props) {
      super(props);
      this.state = {
        hover: false,
        isMouseOver: false
      };
  
      this.onMouseEnter = this.onMouseEnter.bind(this);
      this.onMouseLeave = this.onMouseLeave.bind(this);
      this.onMouseOver = this.onMouseOver.bind(this);
      this.onMouseOut = this.onMouseOut.bind(this);
    }
    onMouseEnter(){
      this.setState({isMouseOver: true});
    }
    onMouseLeave(){
      this.setState({isMouseOver: false});
    }
    onMouseOver(){
      let _this = this;
      setTimeout(function(){ if(_this.state.isMouseOver){ _this.setState({hover: true});}},30);
    }
    onMouseOut(){
      let _this = this;
      setTimeout(function(){ if(!_this.state.isMouseOver){_this.setState({hover: false});}},30);
    }
    render() {
      return (
        <IconButton
            color={this.state.hover ? '#333' : "#D8D8D8"}
            size={10}
            iconName={'CrossIcon'}
            isFill={true}
            isDisabled={this.props.isDisabled}
            onClick={!this.props.isDisabled ? ((e) => this.props.onClick()) : undefined}

            onMouseOver={this.onMouseOver} 
            onMouseOut={this.onMouseOut} 
            onMouseEnter={this.onMouseEnter}
            onMouseLeave={this.onMouseLeave}
        />
      );
    }
  }
  export default CloseButton