import React from 'react'
import styled from 'styled-components'
import device from '../../device'
import { Icons } from '../../icons'

const StyledSectionToggler = styled.div`
  height: 64px;
  display: none;

  @media ${device.tablet} {
    display: ${props => props.visible ? 'block' : 'none'};
  }

  div {
    width: 48px;
    height: 48px;
    padding: 12px 13px 14px 15px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    border-radius: 48px;
    cursor: pointer;
  }
`;


const SectionToggler = (props) => { 
  //console.log("SectionToggler render");
  const { visible, onClick } = props;

  return (
    <StyledSectionToggler visible={visible}>
      <div onClick={onClick}>
        <Icons.CatalogButtonIcon size="scale"/>
      </div>
    </StyledSectionToggler>
  );
}

export default SectionToggler;