import PropTypes from "prop-types";
import React from "react";

import Checkbox from "../checkbox";
import ContextMenuButton from "../context-menu-button";
import ContextMenu from "../context-menu";
import {
  StyledOptionButton,
  StyledContentElement,
  StyledElement,
  StyledCheckbox,
  StyledContent,
  StyledRow,
} from "./styled-row";
import Loader from "../loader";

import { isMobile } from "react-device-detect"; //TODO: isDesktop=true for IOS(Firefox & Safari)

class Row extends React.Component {
  constructor(props) {
    super(props);

    this.cm = React.createRef();
    this.row = React.createRef();
  }

  render() {
    const {
      checked,
      children,
      contentElement,
      contextButtonSpacerWidth,
      data,
      element,
      indeterminate,
      onSelect,
      rowContextClick,
      rowContextClose,
      sectionWidth,
      getContextModel,
      isRoom,
    } = this.props;

    const renderCheckbox = Object.prototype.hasOwnProperty.call(
      this.props,
      "checked"
    );

    const renderElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "element"
    );

    const renderContentElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "contentElement"
    );

    const contextData = data.contextOptions ? data : this.props;

    const renderContext =
      Object.prototype.hasOwnProperty.call(contextData, "contextOptions") &&
      contextData &&
      contextData.contextOptions &&
      contextData.contextOptions.length > 0;

    const changeCheckbox = (e) => {
      onSelect && onSelect(e.target.checked, data);
    };

    const getOptions = () => {
      rowContextClick && rowContextClick();
      return contextData.contextOptions;
    };

    const onContextMenu = (e) => {
      rowContextClick && rowContextClick(e.button === 2);
      if (!this.cm.current.menuRef.current) {
        this.row.current.click(e); //TODO: need fix context menu to global
      }
      this.cm.current.show(e);
    };

    let contextMenuHeader = {};
    if (children.props.item) {
      contextMenuHeader = {
        icon: children.props.item.icon,
        title: children.props.item.title,
      };
    }

    const { onRowClick, inProgress, mode, ...rest } = this.props;

    const onElementClick = () => {
      if (!isMobile) return;

      onSelect && onSelect(true, data);
    };

    return (
      <StyledRow
        ref={this.row}
        {...rest}
        mode={mode}
        onContextMenu={onContextMenu}
      >
        {inProgress ? (
          <Loader className="row-loader" type="oval" size="16px" />
        ) : (
          <>
            {mode == "default" && renderCheckbox && (
              <StyledCheckbox className="not-selectable">
                <Checkbox
                  className="checkbox"
                  isChecked={checked}
                  isIndeterminate={indeterminate}
                  onChange={changeCheckbox}
                />
              </StyledCheckbox>
            )}
            {mode == "modern" && renderCheckbox && renderElement && (
              <StyledCheckbox
                className="not-selectable styled-checkbox-container"
                checked={checked}
                mode={mode}
              >
                <StyledElement
                  onClick={onElementClick}
                  className="styled-element"
                >
                  {element}
                </StyledElement>
                <Checkbox
                  className="checkbox"
                  isChecked={checked}
                  isIndeterminate={indeterminate}
                  onChange={changeCheckbox}
                />
              </StyledCheckbox>
            )}
          </>
        )}
        {mode == "default" && renderElement && (
          <StyledElement onClick={onRowClick} className="styled-element">
            {element}
          </StyledElement>
        )}

        <StyledContent onClick={onRowClick} className="row_content">
          {children}
        </StyledContent>
        <StyledOptionButton
          className="row_context-menu-wrapper"
          spacerWidth={contextButtonSpacerWidth}
        >
          {renderContentElement && (
            <StyledContentElement>{contentElement}</StyledContentElement>
          )}
          {renderContext ? (
            <ContextMenuButton
              className="expandButton"
              getData={getOptions}
              directionX="right"
              isNew={true}
              onClick={onContextMenu}
            />
          ) : (
            <div className="expandButton"> </div>
          )}
          <ContextMenu
            getContextModel={getContextModel}
            model={contextData.contextOptions}
            ref={this.cm}
            header={contextMenuHeader}
            withBackdrop={true}
            onHide={rowContextClose}
            isRoom={isRoom}
          ></ContextMenu>
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
  /** On click anywhere in the row, except the checkbox and context menu */
  onRowClick: PropTypes.func,
  rowContextClick: PropTypes.func,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  inProgress: PropTypes.bool,
  getContextModel: PropTypes.func,
  mode: PropTypes.string,
};

Row.defaultProps = {
  contextButtonSpacerWidth: "26px",
  mode: "default",
  data: {},
};

export default Row;
