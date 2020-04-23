import styled, { css } from 'styled-components';
import { utils } from 'asc-web-components';

const { mobile } = utils.device;

const StyledFilterInput = styled.div`
    width: 100%;
    min-width: 255px;
    &:after {
        content: " ";
        display: block;
        height: 0;
        clear: both;
        visibility: hidden;
    }

    .styled-search-input {
      display: block;
      float: left;
      width: calc(100% - 140px);
      @media ${mobile} {
          width: calc(100% - 58px);
      }

      .search-input-block {
          & > input { 
              height: 30px; 
          }
      }
    }

    .styled-filter-block {
      display: flex;

      .filter-button {

          svg {
              path:not(:first-child) {
                  stroke: #A3A9AE;
              }
          }

          stroke: #A3A9AE;
          div:active {
              svg path:first-child { 
                  fill: #ECEEF1; 
                  stroke: #A3A9AE;
              }
          }
          div:first-child:hover {
              svg path:not(:first-child) { 
                  stroke: #A3A9AE; 
              }
          }
      }
    }

    .styled-close-button {
      margin-left: 7px;
      margin-top: -1px;
    }

    .styled-filter-block {
      display: flex;
      align-items: center;
    }

    .styled-combobox {
      display: inline-block;
      background: transparent;
      max-width: 185px;
      cursor: pointer;
      vertical-align: middle;
      margin-top: -2px;
      > div:first-child{
        width: auto;
        padding-left: 4px;
      }
      > div:last-child{
        max-width: 220px;
      }
      .combo-button-label {
        color: #333;
      }
    }

    .styled-filter-name {
      line-height: 18px;
      margin-left: 5px;
      margin-top: -2px;
      margin-right: 4px;
    }

    .styled-hide-filter {
      display: inline-block;
      height: 100%;
    }

    .dropdown-style {
      position: relative;

      .drop-down {
        padding: 16px;
      }
    }

    .styled-sort-combobox {
      display: block;
      float: left;
      width: 132px;
      margin-left: 8px;

      @media ${mobile} {
          width: 50px;
          .optionalBlock ~ div:first-child{
              opacity: 0
          }
      }

      .combo-button-label {
          color: #333;
      }
    }


`;

export const StyledFilterItem = styled.div`
  display:  ${props => props.block ? 'flex' : 'inline-block'};
  margin-bottom: ${props => props.block ? '8px' : '0'};
  position: relative;
  height: 100%;
  margin-right: 2px;
  border: 1px solid #ECEEF1;
  border-radius: 3px;
  background-color: #F8F9F9;
  padding-right: 22px;
  
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  box-sizing: border-box;
  color: #555F65;

  &:last-child{
    margin-bottom: 0;
  }
`;

export const StyledFilterItemContent = styled.div`
  display: flex;
  padding: 5px 4px 2px 7px;
  width: 100%;
  user-select: none;
  color: #333;
  ${props =>
    props.isOpen && !props.isDisabled &&
    css`
      background: #ECEEF1;
  `}
  ${props =>
    !props.isDisabled &&
    css`
      &:active{
        background: #ECEEF1;
      }
  `}
`;

export const StyledCloseButtonBlock = styled.div`
  display: flex;
  cursor: ${props =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};
  align-items: center;
  position: absolute;
  height: 100%;
  width: 25px;
  border-left: 1px solid #ECEEF1;
  right: 0;
  top: 0;
  background-color: #F8F9F9;
  ${props =>
    !props.isDisabled &&
    css`
      &:active{
        background: #ECEEF1;
        svg path:first-child { 
          fill: #A3A9AE; 
        }
      }
  `}
`;

export const Caret = styled.div`
  width: 7px;
  position: absolute;
  right: 6px;
  transform: ${props => (props.isOpen ? "rotate(180deg)" : "rotate(0)")};
  top: ${props => (props.isOpen ? "2px" : "0")};
`;

export const StyledHideFilterButton = styled.div`
  box-sizing: border-box;
  display: flex;
  position: relative;
  align-items: center;
  font-weight: 600;
  font-size: 16px;
  height: 100%;
  border: 1px solid #eceef1;
  border-radius: 3px;
  background-color: #f8f9f9;
  padding: 0 20px 0 9px;
  margin-right: 2px;
  cursor: ${props => (props.isDisabled ? "default" : "pointer")};
  font-family: Open Sans;
  font-style: normal;

  :hover {
    border-color: ${props => (props.isDisabled ? "#ECEEF1" : "#A3A9AE")};
  }
  :active {
    background-color: ${props => (props.isDisabled ? "#F8F9F9" : "#ECEEF1")};
  }
`;

export const StyledIconButton = styled.div`
    transform: ${state => !state.sortDirection ? 'scale(1, -1)' : 'scale(1)'};
`;

export default StyledFilterInput;