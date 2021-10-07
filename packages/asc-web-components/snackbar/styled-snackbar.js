import styled from "styled-components";
import Box from "../box";

const StyledSnackBar = styled(Box)`
  transition: all 500ms ease;
  transition-property: top, right, bottom, left, opacity;
  font-family: Open Sans, sans-serif, Arial;
  font-size: 12px;
  min-height: 14px;
  position: relative;
  display: flex;
  align-items: flex-start;
  color: white;
  line-height: 16px;
  padding: 12px;
  margin: 0 0 8px 0;
  opacity: ${(props) => props.opacity || 0};
  width: 100%;
  background-color: ${(props) => props.backgroundColor};
  background-image: url(${(props) => props.backgroundImg || ""});
  border-radius: 6px;

  .logo {
    padding-right: 10px;
  }

  .text-container {
    width: 100%;
    display: flex;
    flex-direction: column;
    gap: 5px;
    text-align: ${(props) => props.textAlign};

    .text-header {
      margin: 0;
    }

    .text-body {
      width: 100%;
      display: flex;
      flex-direction: row;
      gap: 10px;
      justify-content: ${(props) => props.textAlign};
    }
  }

  .action {
    background: inherit;
    display: inline-block;
    border: none;
    font-size: inherit;
    color: "#000";
    margin: 0 0 0 24px;
    padding: 0;
    min-width: min-content;
    cursor: pointer;
    margin-left: auto;
    padding-left: 8px;
    text-decoration: underline;
  }

  .button {
    background: inherit;
    border: none;
    font-size: 13px;
    color: "#000";
    cursor: pointer;
    line-height: 14px;
    text-decoration: underline;
  }
`;

export default StyledSnackBar;
