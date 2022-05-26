import styled, { css }  from "styled-components";
import { isMobile } from "react-device-detect";

const StyledSection = styled.div`
    width: 100%;
    background-color: ${props => props.theme.background};
`;

const Icon = styled.div`
    width: 14px;
    height: 14px;
    background-color: ${props => props.theme.loaders};
`;

const Name = styled.div`
    width: 24px;
    height: 24px;
    background-color: ${props => props.theme.loaders};
    margin-left: 10px;
`;

const MainLoader = styled.div`
    background-color: ${props => props.theme.loaders};
    width: 100%;
    margin-left: 10px;
    height: 13px;
`;

const FilterLoader = styled.div`
    height: 30px;
    background-color: ${props => props.theme.loaders};
`;

const SectionContent = styled.div`
    display: flex;
    align-items: baseline;
    margin: 20px 0;
`;

const SectionWrapper = styled.div`
    padding: 0px 20px;
`;

const SectionHeader = styled.h2`
    color: ${props => props.theme.color};
`;

const StyledIframe = styled.iframe`
    border: none;
    height: 60px;
    width: 100%;
`;

const StyledAction = styled.div`
  position: absolute;
  right: 8px;
  top: 10px;
  background: inherit;
  display: inline-block;
  border: none;
  font-size: inherit;
  color: "#333";
  cursor: pointer;
  text-decoration: underline;
  ${isMobile &&
  css`
    right: 14px;
  `};
`;

export {StyledSection, Icon, Name, MainLoader, FilterLoader, SectionContent, SectionHeader, SectionWrapper, StyledIframe, StyledAction}