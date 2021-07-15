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
  background-color: #f8f7bf;
  border-radius: 6px;

  .logo {
    padding-right: 10px;
  }

  .text-container {
    display: flex;
    flex-direction: column;
    gap: 5px;

    .text-header {
      margin: 0;
    }
  }

  .action {
    background: inherit;
    display: inline-block;
    border: none;
    font-size: inherit;
    color: #0f4071;
    margin: 0 0 0 24px;
    padding: 0;
    min-width: min-content;
    cursor: pointer;
    margin-left: auto;
    padding-left: 8px;
  }
`;

export default StyledSnackBar;
