import Checkbox from "../checkbox";
import ContextMenuButton from "../context-menu-button";
import PropTypes from "prop-types";
import React from "react";
import isEqual from "lodash/isEqual";
import styled from "styled-components";
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
  display: flex;
  width: ${props => props.spacerWidth && props.spacerWidth};

  .expandButton > div:first-child {
    padding: 8px 8px 8px 16px;
  }
`;

class Row extends React.Component {
  constructor(props){
    super(props);

    this.rowRef = React.createRef();
  }

  shouldComponentUpdate(nextProps) {
    if (this.props.needForUpdate) {
      return this.props.needForUpdate(this.props, nextProps);
    }
    return !isEqual(this.props, nextProps);
  }

  componentDidMount() {
    if(this.props.selectItem) {
      this.container = this.rowRef.current;
      this.container.addEventListener('contextmenu', this.onSelectItem);
    }
  }

  componentWillUnmount() {
    this.props.selectItem && this.container.removeEventListener('contextmenu', this.onSelectItem);
  }

  onSelectItem = () => this.props.selectItem && this.props.selectItem();

  render() {
    //console.log("Row render");
    const {
      checked,
      children,
      contextButtonSpacerWidth,
      contextOptions,
      data,
      element,
      indeterminate,
      onSelect,
      selectItem
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
      <StyledRow ref={this.rowRef} {...this.props}>
        {renderCheckbox && (
          <StyledCheckbox>
            <Checkbox isChecked={checked} isIndeterminate={indeterminate} onChange={changeCheckbox} />
          </StyledCheckbox>
        )}
        {renderElement && <StyledElement>{element}</StyledElement>}
        <StyledContent className="row_content">{children}</StyledContent>
        <StyledOptionButton className="row_context-menu-wrapper" spacerWidth={contextButtonSpacerWidth}>
          {renderContext
            ? (<ContextMenuButton isFill color='#A3A9AE' hoverColor='#657077' onClick={selectItem} className="expandButton" directionX="right" getData={getOptions} />)
            : (<div className="expandButton">{' '}</div>)}
        </StyledOptionButton>
      </StyledRow>
    );
  }
}

Row.propTypes = {
  checked: PropTypes.bool,
  children: PropTypes.element,
  className: PropTypes.string,
  contextButtonSpacerWidth: PropTypes.string,
  contextOptions: PropTypes.array,
  data: PropTypes.object,
  element: PropTypes.element,
  id: PropTypes.string,
  indeterminate: PropTypes.bool,
  needForUpdate: PropTypes.func,
  onSelect: PropTypes.func,
  selectItem: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

Row.defaultProps = {
  contextButtonSpacerWidth: '32px'
};

export default Row;
