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
      rowContextClose,
      sectionWidth,
      getContextModel,
      isRoom,
      withoutBorder,
      contextTitle,
    } = this.props;

    const { onRowClick, inProgress, mode, onContextClick, ...rest } =
      this.props;

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
      onContextClick && onContextClick();
      return contextData.contextOptions;
    };

    const onContextMenu = (e) => {
      onContextClick && onContextClick(e.button === 2);
      if (!this.cm.current.menuRef.current) {
        this.row.current.click(e); //TODO: need fix context menu to global
      }
      this.cm.current.show(e);
    };

    let contextMenuHeader = {};
    if (children.props.item) {
      contextMenuHeader = {
        icon: children.props.item.icon,
        avatar: children.props.item.avatar,
        title: children.props.item.title
          ? children.props.item.title
          : children.props.item.displayName,
      };
    }

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
        withoutBorder={withoutBorder}
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
              displayType="toggle"
              onClick={onContextMenu}
              title={contextTitle}
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
  /** Required for hosting the Checkbox component. Its location is always fixed in the first position.
   * If there is no value, the occupied space is distributed among the other child elements. */
  checked: PropTypes.bool,
  /** Displays the child elements */
  children: PropTypes.element,
  /** Accepts class */
  className: PropTypes.string,
  /** Required for displaying a certain element in the row */
  contentElement: PropTypes.any,
  /** Sets the width of the ContextMenuButton component. */
  contextButtonSpacerWidth: PropTypes.string,
  /** Required for hosting the ContextMenuButton component. It is always located near the right border of the container,
   * regardless of the contents of the child elements. If there is no value, the occupied space is distributed among the other child elements. */
  contextOptions: PropTypes.array,
  /** Current row item information. */
  data: PropTypes.object,
  /** In case Checkbox component is specified, it is located in a fixed order,
   * otherwise it is located in the first position. If there is no value, the occupied space is distributed among the other child elements. */
  element: PropTypes.element,
  /** Accepts id  */
  id: PropTypes.string,
  /** If true, this state is shown as a rectangle in the checkbox */
  indeterminate: PropTypes.bool,
  /** Sets a callback function that is triggered when a row element is selected. Returns data value. */
  onSelect: PropTypes.func,
  /** Sets a callback function that is triggered when any element except the checkbox and context menu is clicked. */
  onRowClick: PropTypes.func,
  /** Function that is invoked on clicking the icon button in the context-menu */
  onContextClick: PropTypes.func,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Width section */
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** Displays the loader*/
  inProgress: PropTypes.bool,
  /** Function that returns an object containing the elements of the context menu */
  getContextModel: PropTypes.func,
  /** Changes the row mode */
  mode: PropTypes.string,
  /** Removes the borders */
  withoutBorder: PropTypes.bool,
};

Row.defaultProps = {
  contextButtonSpacerWidth: "26px",
  mode: "default",
  data: {},
  withoutBorder: false,
};

export default Row;
