import styled from "styled-components";

const StyledBreakpointWarning = styled.div`
  padding: 44px 44px 0 44px;

  .description {
    display: flex;
    flex-direction: column;
    padding-top: 36px;
    white-space: break-spaces;
  }

  .text-breakpoint {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
    padding-bottom: 8px;
  }

  .text-prompt {
    font-weight: 400;
    font-size: 12px;
    line-height: 16px;
  }
`;

export default StyledBreakpointWarning;
