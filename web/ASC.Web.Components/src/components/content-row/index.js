import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'


const StyledContentRow = styled.div`
    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 12px;
    line-height: 16px;
    color: #A3A9AE;
    
    cursor: default;

    min-height: 47px;
    width: 100%;
    border-bottom: 1px solid #ECEEF1;

    display: flex;
    flex-direction: row;
    flex-wrap: nowrap;

    justify-content: flex-start;
    align-items: center;
    align-content: center;
    }
`;

const StyledContent = styled.div`
    display: flex;
    flex-basis: 100%;

    & > a, p {
        margin-left: 16px;
    }

    &.no-gutters {
        margin-right: 0;
        margin-left: 0;

        overflow:auto;
        white-space:nowrap;
      
        > .col,
        > [class*="col-"] {
          padding-right: 0;
          padding-left: 0;
        }
      }
`;

const StyledCheckbox = styled.div`
    flex-basis: 16px;
    display: flex;
    margin-right: 16px;
    margin-bottom: 0px;
    margin-top: 8px;
`;

const StyledAvatar = styled.div`
    flex: 0 0 32px;
    display: flex;
`;

const StyledOptionButton = styled.div`
    flex: 0 0 auto;
    display: flex;
    margin-left: 16px;
`;

const ContentRow = props => {
    const { checkBox, avatar, contextButton, children } = props;
    return (
        <StyledContentRow {...props}>
            {checkBox && <StyledCheckbox>{checkBox}</StyledCheckbox>}
            {avatar && <StyledAvatar>{avatar}</StyledAvatar>}
            <StyledContent>{children}</StyledContent>
            {contextButton && <StyledOptionButton>{contextButton}</StyledOptionButton>}
        </StyledContentRow>
    );
}

ContentRow.propTypes = {
    checkBox: PropTypes.element,
    avatar: PropTypes.element,
    contextButton: PropTypes.element
};

ContentRow.defaultProps = {
};

export default ContentRow;