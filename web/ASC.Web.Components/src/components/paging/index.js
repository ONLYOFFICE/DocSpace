import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'

import Button from '../button'
import ComboBox from '../combobox'
import device from '../device'


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

  @media ${device.mobile} {
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
    openDirection, disablePrevious, disableNext, selectedPage, selectedCount, emptyPagePlaceholder, 
    emptyCountPlaceholder } = props;

  const onSelectPageAction = (option) => {
    props.onSelectPage(option);
  }

  const onSelectCountAction = (option) => {
    props.onSelectCount(option)
  }

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
            selectedOption={selectedPage} 
            dropDownMaxHeight="200px"
            emptyOptionsPlaceholder={emptyPagePlaceholder} />
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
            selectedOption={selectedCount}
            emptyOptionsPlaceholder={emptyCountPlaceholder} />
        </StyledOnPage>
      }
    </StyledPaging>
  );
};

Paging.propTypes = {
  previousLabel: PropTypes.string,
  previousAction: PropTypes.func,
  disablePrevious: PropTypes.bool,
  nextLabel: PropTypes.string,
  nextAction: PropTypes.func,
  disableNext: PropTypes.bool,
  selectedPage: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  selectedCount: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  onSelectPage: PropTypes.func,
  onSelectCount: PropTypes.func,
  emptyPagePlaceholder: PropTypes.string,
  emptyCountPlaceholder: PropTypes.string
}

Paging.defaultProps = {
  previousAction: previousAction,
  nextAction: nextAction,
  disablePrevious: false,
  disableNext: false,
  emptyPagePlaceholder: '1 of 1',
  emptyCountPlaceholder: '25 per page'
}

export default Paging;