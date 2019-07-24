import React from 'react'
import styled from 'styled-components'
import device from '../../device'
import { Icons } from '../../icons'

const StyledArticlePinPanel = styled.div`
  border-top: 1px solid #ECEEF1;
  height: 56px;
  display: none;

  @media ${device.tablet} {
    display: block;
  }

  @media ${device.mobile} {
    display: none;
  }

  div {
    display: flex;
    align-items: center;
    cursor: pointer;
    user-select: none;
    height: 100%;

    span {
      margin-left: 8px;
    }
  }
`;

const ArticlePinPanel = (props) => { 
  //console.log("ArticlePinPanel render");
  const { pinned, pinText, onPin, unpinText, onUnpin } = props;

  return (
    <StyledArticlePinPanel>
      {
        pinned
          ? <div onClick={onUnpin}>
              <Icons.CatalogUnpinIcon size="medium"/>
              <span>{unpinText}</span>
            </div>
          : <div onClick={onPin}>
              <Icons.CatalogPinIcon size="medium"/>
              <span>{pinText}</span>
            </div>
      }
    </StyledArticlePinPanel>
  );
}

export default ArticlePinPanel;