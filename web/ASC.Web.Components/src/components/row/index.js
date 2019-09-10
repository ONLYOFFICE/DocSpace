import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'

import Checkbox from '../checkbox'
import ContextMenuButton from '../context-menu-button'

const StyledRow = styled.div`
  cursor: default;
    
  min-height: 50px;
  width: 100%;
  border-bottom: 1px solid #ECEEF1;

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

  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const StyledCheckbox = styled.div`
  flex: 0 0 16px;
`;

const StyledElement = styled.div`
  flex: 0 0 auto;
  display: flex;
  margin-right: 8px;
  user-select: none;
`;

const StyledOptionButton = styled.div`
  flex: 0 0 18px;
  display: flex;
  margin-left: 8px;
  margin-right: 16px;
`;

class Row extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      checked: this.props.checked
    }
  }

  changeCheckbox = (e) => {
    this.props.onSelect && this.props.onSelect(e.target.checked, this.props.data);
  };

  getOptions = () => this.props.contextOptions;

  componentDidUpdate(prevProps) {
    if (this.props.checked !== prevProps.checked) {
      this.setState({ checked: this.props.checked });
    }
  }
  
  render() {
    //console.log("Row render");
    const { checked, element, children, contextOptions } = this.props;

    return (
      <StyledRow ref={this.rowRef} {...this.props}>
        {this.props.hasOwnProperty("checked") &&
          <StyledCheckbox>
            <Checkbox isChecked={checked} onChange={this.changeCheckbox} />
          </StyledCheckbox>
        }
        {this.props.hasOwnProperty("element") &&
          <StyledElement>
            {element}
          </StyledElement>
        }
        <StyledContent>
          {children}
        </StyledContent>
        <StyledOptionButton>
          {this.props.hasOwnProperty("contextOptions") && contextOptions.length > 0 &&
            <ContextMenuButton directionX='right' getData={this.getOptions} />
          }
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
  onSelect: PropTypes.func
};

export default Row;