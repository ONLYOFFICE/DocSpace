import React from "react";
import ContextMenuButton from '../context-menu-button';

class FilterButton extends React.PureComponent{
  render(){
    //console.log('render FilterButton)
    return(
      <ContextMenuButton
        title='Actions'
        iconName='RectangleFilterIcon'
        color='#A3A9AE'
        size={this.props.iconSize}
        isDisabled={this.props.isDisabled}
        getData={this.props.getData}
        iconHoverName='RectangleFilterHoverIcon'
        iconClickName='RectangleFilterClickIcon'
      ></ContextMenuButton>
    )
  }
}

export default FilterButton