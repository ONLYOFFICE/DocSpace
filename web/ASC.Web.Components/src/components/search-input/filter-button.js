import React from "react";
import ContextMenuButton from '../context-menu-button';

const FilterButton = props => {
  //console.log("FilterButton render");
  return (
    <ContextMenuButton
          title={'Actions'}
          iconName={'RectangleFilterIcon'}
          color='#A3A9AE'
          size={props.iconSize}
          isDisabled={props.isDisabled}
          getData={props.getData}
          iconHoverName={'RectangleFilterHoverIcon'}
          iconClickName={'RectangleFilterClickIcon'}
      ></ContextMenuButton>
  );
};

export default FilterButton