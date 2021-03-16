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

    this.state = { contextX: "0px", contextY: "0px", contextOpened: false };

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

    const cursorX = -(window.innerWidth - e.pageX) + "px";
    const cursorY = "-3px";

    this.setState({
      contextX: cursorX,
      contextY: cursorY,
      contextOpened: !this.state.contextOpened,
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
              opened={this.state.contextOpened}
              color="#A3A9AE"
              hoverColor="#657077"
              className="expandButton"
              manualX={this.state.contextX}
              manualY={this.state.contextY}
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
