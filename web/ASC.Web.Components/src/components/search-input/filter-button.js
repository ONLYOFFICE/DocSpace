import React from "react";
import ContextMenuButton from '../context-menu-button';

class FilterButton extends React.Component {
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
      this.setState({isMouseOver: true})
    }
    onMouseLeave(){
      this.setState({isMouseOver: false})
    }
    onMouseOver(){
      let _this = this;
      setTimeout(function(){ if(_this.state.isMouseOver){ _this.setState({hover: true})}},30);
    }
    onMouseOut(){
      let _this = this;
      setTimeout(function(){ if(!_this.state.isMouseOver){_this.setState({hover: false})}},30);
    }
    render() {
      return (
        <ContextMenuButton
          title={'Actions'}
          iconName={this.state.hover ? 'RectangleFilterHoverIcon' : 'RectangleFilterIcon'}
          color='#A3A9AE'
          size={this.props.iconSize}
          isDisabled={this.props.isDisabled}
          getData={this.props.getData}
          onMouseOver={this.onMouseOver} 
          onMouseOut={this.onMouseOut} 
          onMouseEnter={this.onMouseEnter}
          onMouseLeave={this.onMouseLeave}
        ></ContextMenuButton>
      );
    }
  }
  export default FilterButton