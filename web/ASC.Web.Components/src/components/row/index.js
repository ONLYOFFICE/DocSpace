import React, { memo } from 'react'
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
const Row = props => {
  const changeCheckbox = (e) => {
    props.onSelect && props.onSelect(e.target.checked, props.data);
  };

  const getOptions = () => props.contextOptions;
  //console.log("Row render");
  const { checked, element, children, contextOptions } = props;

  return (
    <StyledRow {...props}>
      {Object.prototype.hasOwnProperty.call(props, 'checked') &&
        <StyledCheckbox>
          <Checkbox isChecked={checked} onChange={changeCheckbox} />
        </StyledCheckbox>
      }
      {Object.prototype.hasOwnProperty.call(props, 'element') &&
        <StyledElement>
          {element}
        </StyledElement>
      }
      <StyledContent>
        {children}
      </StyledContent>
      <StyledOptionButton>
        {Object.prototype.hasOwnProperty.call(props, 'contextOptions') && contextOptions.length > 0 &&
          <ContextMenuButton directionX='right' getData={getOptions} />
        }
      </StyledOptionButton>
    </StyledRow>
  );
};

Row.propTypes = {
  checked: PropTypes.bool,
  element: PropTypes.element,
  children: PropTypes.element,
  data: PropTypes.object,
  contextOptions: PropTypes.array,
  onSelect: PropTypes.func
};

export default Row;