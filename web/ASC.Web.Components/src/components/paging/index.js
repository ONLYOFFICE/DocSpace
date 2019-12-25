import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'

import Button from '../button'
import ComboBox from '../combobox'

const StyledPaging = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: flex-start;

  & > button {
    margin-right: 8px;
    max-width: 110px;
  }

  .buttonCustomStyle {
    padding: 6px 8px 10px;
  }
`;

const StyledOnPage = styled.div`
  margin-left: auto;
  margin-right: 0px;

  .hideDisabled {
    div[disabled] {
      display: none;
    }
  }

  @media (max-width: 450px) {
    display: none;
  }
`;

const StyledPage = styled.div`
  margin-right: 8px;

  .manualWidth {
    > div:last-of-type {
        width: 120%;
      }
    }
  }
`;

const Paging = props => {
  //console.log("Paging render");
  const { previousLabel, nextLabel, previousAction, nextAction, pageItems, countItems,
    openDirection, disablePrevious, disableNext, selectedPageItem, selectedCountItem,
    id, className, style } = props;

  const onSelectPageAction = (option) => {
    props.onSelectPage(option);
  }

  const onSelectCountAction = (option) => {
    props.onSelectCount(option)
  }

  const setDropDownMaxHeight = pageItems && pageItems.length > 6 ? { dropDownMaxHeight: 200 } : {};

  return (
    <StyledPaging
      id={id}
      className={className}
      style={style}
    >
      <Button
        className="buttonCustomStyle"
        size='medium'
        scale={true}
        label={previousLabel}
        onClick={previousAction}
        isDisabled={disablePrevious} />
      {pageItems &&
        <StyledPage>
          <ComboBox
            className="manualWidth"
            directionY={openDirection}
            options={pageItems}
            onSelect={onSelectPageAction}
            scaledOptions={pageItems.length > 6}
            selectedOption={selectedPageItem}
            {...setDropDownMaxHeight} />
        </StyledPage>
      }
      <Button
        className="buttonCustomStyle"
        size='medium'
        scale={true}
        label={nextLabel}
        onClick={nextAction}
        isDisabled={disableNext} />
      {countItems &&
        <StyledOnPage>
          <ComboBox
            className="hideDisabled"
            directionY={openDirection}
            directionX='right'
            options={countItems}
            onSelect={onSelectCountAction}
            selectedOption={selectedCountItem} />
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
  onSelectCount: PropTypes.func,

  pageItems: PropTypes.array,
  countItems: PropTypes.array,

  openDirection: PropTypes.oneOf(['bottom', 'top']),

  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
}

Paging.defaultProps = {
  disablePrevious: false,
  disableNext: false
}

export default Paging;