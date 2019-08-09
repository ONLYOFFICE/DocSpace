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

    & > button, div:not(:last-of-type) {
        margin-right: 8px;
    }
`;

const StyledOnPage = styled.div`
    width: 120px;
    margin-left: auto;

    @media ${device.mobile} {
        display: none;
    }
`;

const StyledPage = styled.div`

`;

const previousAction = () => {
  console.log('Prev action');
};

const nextAction = () => {
  console.log('Next action');
};

const Paging = props => {
  //console.log("Paging render");
  const { previousLabel, nextLabel, previousAction, nextAction, pageItems, countItems, openDirection, disablePrevious, disableNext, selectedPage, selectedCount } = props;

  const onSelectPageAction = (option) => {
    props.onSelectPage(option);
  }

  const onSelectCountAction = (option) => {
    props.onSelectCount(option)
  }

  return (
    <StyledPaging>
      <Button size='medium' label={previousLabel} onClick={previousAction} isDisabled={disablePrevious} />
      {pageItems &&
        <StyledPage>
          <ComboBox directionY={openDirection} options={pageItems} onSelect={onSelectPageAction} selectedOption={selectedPage} dropDownMaxHeight="200px" />
        </StyledPage>
      }
      <Button size='medium' label={nextLabel} onClick={nextAction} isDisabled={disableNext} />
      {countItems &&
        <StyledOnPage>
          <ComboBox directionY={openDirection} options={countItems} onSelect={onSelectCountAction} selectedOption={selectedCount} />
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
  onSelectCount: PropTypes.func
}

Paging.defaultProps = {
  previousAction: previousAction,
  nextAction: nextAction,
  disablePrevious: false,
  disableNext: false
}

export default Paging;