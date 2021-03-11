import PropTypes from "prop-types";
import React from "react";
import equal from "fast-deep-equal/react";

import Checkbox from "../checkbox";
import ContextMenuButton from "../context-menu-button";
import {
  StyledOptionButton,
  StyledContentElement,
  StyledElement,
  StyledCheckbox,
  StyledContent,
  StyledRow,
} from "./styled-row";

class Row extends React.Component {
  constructor(props) {
    super(props);

    this.rowRef = React.createRef();
  }

  // shouldComponentUpdate(nextProps) {
  //   if (this.props.needForUpdate) {
  //     return this.props.needForUpdate(this.props, nextProps);
  //   }
  //   return !equal(this.props, nextProps);
  // }

  componentDidMount() {
    if (this.props.selectItem) {
      this.container = this.rowRef.current;
      this.container.addEventListener("contextmenu", this.onSelectItem);
    }
  }

  componentWillUnmount() {
    this.props.selectItem &&
      this.container.removeEventListener("contextmenu", this.onSelectItem);
  }

  onSelectItem = () => this.props.selectItem && this.props.selectItem();

  render() {
    //console.log("Row render");
    const {
      checked,
      children,
      contentElement,
      contextButtonSpacerWidth,
      contextOptions,
      data,
      element,
      indeterminate,
      onSelect,
      selectItem,
      sectionWidth,
    } = this.props;

    const renderCheckbox = Object.prototype.hasOwnProperty.call(
      this.props,
      "checked"
    );

    const renderElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "element"
    );

    const renderContentElement =
      Object.prototype.hasOwnProperty.call(this.props, "contentElement") &&
      sectionWidth > 500;

    const renderContext =
      Object.prototype.hasOwnProperty.call(this.props, "contextOptions") &&
      contextOptions.length > 0;

    const changeCheckbox = (e) => {
      onSelect && onSelect(e.target.checked, data);
    };

    const getOptions = () => contextOptions;

    return (
      <StyledRow ref={this.rowRef} {...this.props}>
        {renderCheckbox && (
          <StyledCheckbox>
            <Checkbox
              isChecked={checked}
              isIndeterminate={indeterminate}
              onChange={changeCheckbox}
            />
          </StyledCheckbox>
        )}
        {renderElement && (
          <StyledElement className="styled-element">{element}</StyledElement>
        )}
        <StyledContent className="row_content">{children}</StyledContent>
        <StyledOptionButton
          className="row_context-menu-wrapper"
          spacerWidth={contextButtonSpacerWidth}
        >
          {renderContentElement && (
            <StyledContentElement onClick={selectItem}>
              {contentElement}
            </StyledContentElement>
          )}
          {renderContext ? (
            <ContextMenuButton
              isFill
              color="#A3A9AE"
              hoverColor="#657077"
              onClick={selectItem}
              className="expandButton"
              directionX="right"
              getData={getOptions}
            />
          ) : (
            <div className="expandButton"> </div>
          )}
        </StyledOptionButton>
      </StyledRow>
    );
  }
}

Row.propTypes = {
  /** Required to host the Checkbox component. Its location is fixed and it is always the first.
   * If there is no value, the occupied space is distributed among the other child elements. */
  checked: PropTypes.bool,
  children: PropTypes.element,
  /** Accepts class */
  className: PropTypes.string,
  contentElement: PropTypes.any,
  /** Required for the width task of the ContextMenuButton component. */
  contextButtonSpacerWidth: PropTypes.string,
  /** Required to host the ContextMenuButton component. It is always located near the right border of the container,
   * regardless of the contents of the child elements. If there is no value, the occupied space is distributed among the other child elements. */
  contextOptions: PropTypes.array,
  /** Current row item information. */
  data: PropTypes.object,
  /** Required to host some component. It has a fixed order of location, if the Checkbox component is specified,
   * then it follows, otherwise it occupies the first position. If there is no value, the occupied space is distributed among the other child elements. */
  element: PropTypes.element,
  /** Accepts id  */
  id: PropTypes.string,
  indeterminate: PropTypes.bool,
  /** shouldComponentUpdate function  */
  needForUpdate: PropTypes.func,
  /** when selecting row element. Returns data value. */
  onSelect: PropTypes.func,
  selectItem: PropTypes.func,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
};

Row.defaultProps = {
  contextButtonSpacerWidth: "26px",
};

export default Row;
