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
    width: 80px;
`;

const Paging = props => {
    const {previousLabel, nextLabel, previousAction, nextAction, pageItems, perPageItems, openDirection} = props;
    
    return (
        <StyledPaging>
            {pageItems && 
            <>
                <Button size='medium' label={previousLabel} onClick={previousAction}/>
                <StyledPage>
                    <ComboBox directionY={openDirection} items={pageItems} />
                </StyledPage>
                <Button size='medium' label={nextLabel} onClick={nextAction}/>
            </>
            }
            <StyledOnPage>
                <ComboBox directionY={openDirection} items={perPageItems} />
            </StyledOnPage>
        </StyledPaging>
    );
};

Paging.propTypes = {
    previousAction: PropTypes.func,
    nextAction: PropTypes.func,
    previousLabel: PropTypes.string,
    nextLabel: PropTypes.string
}

Paging.defaultProps = {
    previousAction: () => console.log('Prev action'),
    nextAction: () => console.log('Next action')
}

export default Paging;