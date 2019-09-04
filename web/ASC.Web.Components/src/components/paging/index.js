import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'

import Button from '../button'
import ComboBox from '../combobox'
import { mobile } from '../../utils/device'


const StyledPaging = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: flex-start;

  & > button {
    margin-right: 8px;
    width: 110px;
  }
`;

const StyledOnPage = styled.div`
  margin-left: auto;
  margin-right: 0px;

  @media ${mobile} {
    display: none;
  }
`;

const StyledPage = styled.div`
  margin-right: 8px;
`;

const previousAction = () => {
  console.log('Prev action');
};

const nextAction = () => {
  console.log('Next action');
};

const Paging = props => {
  //console.log("Paging render");
  const { previousLabel, nextLabel, previousAction, nextAction, pageItems, countItems, 
    openDirection, disablePrevious, disableNext, selectedPageItem, selectedCountItem} = props;

  const onSelectPageAction = (option) => {
    props.onSelectPage(option);
  }

  const onSelectCountAction = (option) => {
    props.onSelectCount(option)
  }

  const setDropDownMaxHeight = pageItems && pageItems.length > 6 ? { dropDownMaxHeight: 200 } : {};

  return (
    <StyledPaging>
      <Button 
        size='medium' 
        scale={true} 
        label={previousLabel} 
        onClick={previousAction} 
        isDisabled={disablePrevious} />
      {pageItems &&
        <StyledPage>
          <ComboBox 
            directionY={openDirection} 
            options={pageItems} 
            onSelect={onSelectPageAction} 
            selectedOption={selectedPageItem}
            {...setDropDownMaxHeight} />
        </StyledPage>
      }
      <Button 
        size='medium' 
        scale={true} 
        label={nextLabel} 
        onClick={nextAction} 
        isDisabled={disableNext} />
      {countItems &&
        <StyledOnPage>
          <ComboBox 
            directionY={openDirection} 
            options={countItems} 
            onSelect={onSelectCountAction} 
            selectedOption={selectedCountItem}/>
        </StyledOnPage>
      }
    </StyledPaging>
  );
};

Paging.propTypes = {
  previousLabel: PropTypes.string,
  nextLabel: PropTypes.string,

  previousAction: PropTypes.func,
  nextAction: PropTypes.func,

  disablePrevious: PropTypes.bool,
  disableNext: PropTypes.bool,

  selectedPageItem: PropTypes.object,
  selectedCountItem: PropTypes.object,

  onSelectPage: PropTypes.func,
  onSelectCount: PropTypes.func
}

Paging.defaultProps = {
  previousAction: previousAction,
  nextAction: nextAction,
  disablePrevious: false,
  disableNext: false
}

export default Paging;