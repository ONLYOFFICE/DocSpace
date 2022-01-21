import React from "react";
import ContextMenuButton from "@appserver/components/context-menu-button";
import PropTypes from "prop-types";

class FilterButton extends React.PureComponent {
  render() {
    const {
      getData,
      id,
      isDisabled,
      iconSize,
      columnCount,
      asideHeader,
      asideView,
    } = this.props;
    //console.log('render FilterButton)
    return (
      <ContextMenuButton
        //className="filter-button"
        directionY="bottom"
        getData={getData}
        iconName="/static/images/rectangle.filter.react.svg"
        iconOpenName="/static/images/rectangle.filter.click.react.svg"
        id={id}
        isDisabled={isDisabled}
        size={iconSize}
        columnCount={columnCount}
        displayType={asideView ? "aside" : "auto"}
        asideHeader={asideHeader}
      ></ContextMenuButton>
    );
  }
}
FilterButton.propTypes = {
  getData: PropTypes.func,
  iconSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
  columnCount: PropTypes.number,
  asideHeader: PropTypes.string,
  asideView: PropTypes.bool,
};
export default FilterButton;
