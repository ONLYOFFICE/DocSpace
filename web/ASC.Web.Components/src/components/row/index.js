import Checkbox from "../checkbox";
import ContextMenuButton from "../context-menu-button";
import PropTypes from "prop-types";
import React from "react";
import styled from "styled-components";
import { tablet } from "../../utils/device";
import equal from "fast-deep-equal/react";

const StyledRow = styled.div`
  cursor: default;

  min-height: 47px;
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

const StyledContentElement = styled.div`
  margin-top: 6px;
  user-select: none;
`;

const StyledOptionButton = styled.div`
  display: flex;
  width: ${(props) => props.spacerWidth && props.spacerWidth};
  justify-content: flex-end;

  .expandButton > div:first-child {
    padding: 8px 0px 9px 7px;

    margin-right: 0px;

    @media (min-width: 1024px) {
      margin-right: -1px;
    }
    @media (max-width: 516px) {
      padding-left: 10px;
    }
  }

  //margin-top: -1px;
  @media ${tablet} {
    margin-top: unset;
  }
`;

class Row extends React.Component {
  constructor(props) {
    super(props);

    this.rowRef = React.createRef();
  }

  shouldComponentUpdate(nextProps) {
    if (this.props.needForUpdate) {
      return this.props.needForUpdate(this.props, nextProps);
    }
    return !equal(this.props, nextProps);
  }

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
  checked: PropTypes.bool,
  children: PropTypes.element,
  className: PropTypes.string,
  contentElement: PropTypes.any,
  contextButtonSpacerWidth: PropTypes.string,
  contextOptions: PropTypes.array,
  data: PropTypes.object,
  element: PropTypes.element,
  id: PropTypes.string,
  indeterminate: PropTypes.bool,
  needForUpdate: PropTypes.func,
  onSelect: PropTypes.func,
  selectItem: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
};

Row.defaultProps = {
  contextButtonSpacerWidth: "26px",
};

export default Row;
