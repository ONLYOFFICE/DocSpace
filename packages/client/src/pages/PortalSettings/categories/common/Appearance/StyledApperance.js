import styled from "styled-components";

const StyledComponent = styled.div`
  padding-top: 3px;
  width: 100%;

  .header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
  }

  .preview-header {
    padding-bottom: 20px;
  }

  .theme-standard {
    padding-top: 21px;
  }

  .theme-name {
    font-size: 15px;
    line-height: 16px;
    font-weight: 600;
  }

  .theme-container {
    padding: 12px 0 24px 0;
    display: flex;
  }
  .box {
    width: 46px;
    height: 46px;
    margin-right: 12px;
    border-radius: 8px;
    cursor: pointer;
  }

  .theme-add {
    background: ${(props) => (props.theme.isBase ? "#eceef1" : "#474747")}
      url("/static/images/plus.theme.svg") no-repeat center;
  }

  .add-theme {
    background: #d0d5da;
    padding-top: 16px;
    padding-left: 16px;
    box-sizing: border-box;
  }

  .buttons-container {
    padding-top: 24px;
  }

  .button:not(:last-child) {
    margin-right: 8px;
  }

  .check-img {
    padding: 18px 0 0 15px;
    svg path {
      fill: ${(props) => props.colorCheckImg};
    }
  }
`;

const StyledTheme = styled.div`
  width: 46px;
  height: 46px;
  margin-right: 12px;
  border-radius: 8px;
  cursor: pointer;

  .check-hover {
    visibility: hidden;
  }

  &:hover {
    .check-hover {
      padding: 18px 0 0 15px;
      visibility: visible;
      opacity: 0.5;
      svg path {
        fill: ${(props) => props.colorCheckImgHover};
      }
    }
  }
`;
export { StyledComponent, StyledTheme };
