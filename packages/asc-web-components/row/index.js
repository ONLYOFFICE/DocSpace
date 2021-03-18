import PropTypes from "prop-types";
import React from "react";

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

    this.state = {
      contextX: "0px",
      contextY: "100%",
      contextOpened: false,
    };

    this.rowRef = React.createRef();
  }

  componentDidMount() {
    this.container = this.rowRef.current;
    this.container.addEventListener("contextmenu", this.onContextMenu);
  }

  componentWillUnmount() {
    this.container &&
      this.container.removeEventListener("contextmenu", this.onContextMenu);
  }

  onContextMenu = (e) => {
    e.preventDefault();

    const menu = document.getElementById("contextMenu");

    const containerBounds =
      this.container !== document && this.container.getBoundingClientRect();

    const clickX = containerBounds.right - e.clientX;
    const clickY = e.clientY - containerBounds.top;
    const containerWidth = this.container.offsetWidth;
    const containerHeight = this.container.offsetHeight;
    const menuWidth = (menu && menu.offsetWidth) || 180;
    const menuHeight = menu && menu.offsetHeight;

    const left = containerWidth - clickX > menuWidth && clickX < menuWidth;
    const bottom = containerHeight - clickY < menuHeight && clickY > menuHeight;

    let newTop = `0px`;
    let newRight = `0px`;

    newRight = !left ? `${clickX - menuWidth - 8}px` : `${clickX + 8}px`;
    newTop = bottom ? `${clickY - menuHeight}px` : `${clickY}px`;

    this.setState({
      contextOpened: !this.state.contextOpened,
      contextX: newRight,
      contextY: newTop,
    });
  };

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
      sectionWidth,
    } = this.props;

    const { contextOpened, contextX, contextY } = this.state;

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
            <StyledContentElement>{contentElement}</StyledContentElement>
          )}
          {renderContext ? (
            <ContextMenuButton
              manualX={contextX}
              manualY={contextY}
              opened={contextOpened}
              color="#A3A9AE"
              hoverColor="#657077"
              className="expandButton"
              getData={getOptions}
              directionX="right"
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
  /** If true, this state is shown as a rectangle in the checkbox */
  indeterminate: PropTypes.bool,
  /** when selecting row element. Returns data value. */
  onSelect: PropTypes.func,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
};

Row.defaultProps = {
  contextButtonSpacerWidth: "26px",
};

export default Row;
