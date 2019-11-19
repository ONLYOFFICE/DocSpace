import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import isEqual from "lodash/isEqual";
import Checkbox from "../checkbox";
import ContextMenuButton from "../context-menu-button";
import { tablet } from "../../utils/device";

const StyledRow = styled.div`
  cursor: default;

  min-height: 50px;
  width: 100%;
  border-bottom: 1px solid #eceef1;

  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;
`;

const StyledContent = styled.div`
  display: flex;
  flex-basis: 100%;

  min-width: 160px;

  @media ${tablet} {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const StyledCheckbox = styled.div`
  flex: 0 0 16px;
`;

const StyledElement = styled.div`
  flex: 0 0 auto;
  display: flex;
  margin-right: 8px;
  margin-left: 2px;
  user-select: none;
`;

const StyledOptionButton = styled.div`
  flex: 0 0 18px;
  display: flex;
  margin-left: 8px;
  margin-right: 16px;
`;

// eslint-disable-next-line react/display-name

class Row extends React.Component {
  shouldComponentUpdate(nextProps) {
    if (this.props.needForUpdate) {
      return this.props.needForUpdate(this.props, nextProps);
    }
    return !isEqual(this.props, nextProps);
  }

  render() {
    //console.log("Row render");
    const {
      checked,
      element,
      children,
      data,
      contextOptions,
      onSelect
    } = this.props;

    const renderCheckbox = Object.prototype.hasOwnProperty.call(
      this.props,
      "checked"
    );

    const renderElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "element"
    );

    const renderContext =
      Object.prototype.hasOwnProperty.call(this.props, "contextOptions") &&
      contextOptions.length > 0;

    const changeCheckbox = e => {
      onSelect && onSelect(e.target.checked, data);
    };

    const getOptions = () => contextOptions;

    return (
      <StyledRow {...this.props}>
        {renderCheckbox && (
          <StyledCheckbox>
            <Checkbox isChecked={checked} onChange={changeCheckbox} />
          </StyledCheckbox>
        )}
        {renderElement && <StyledElement>{element}</StyledElement>}
        <StyledContent>{children}</StyledContent>
        <StyledOptionButton>
          {renderContext && (
            <ContextMenuButton directionX="right" getData={getOptions} />
          )}
        </StyledOptionButton>
      </StyledRow>
    );
  }
}

Row.propTypes = {
  checked: PropTypes.bool,
  element: PropTypes.element,
  children: PropTypes.element,
  data: PropTypes.object,
  contextOptions: PropTypes.array,
  onSelect: PropTypes.func,
  needForUpdate: PropTypes.func
};

export default Row;
