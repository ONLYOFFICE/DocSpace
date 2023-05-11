import React, { ForwardedRef, forwardRef } from "react";
import { isDesktop } from "react-device-detect";
import styled from "styled-components";

const Wrapper = styled.section`
  width: 100%;
  height: ${`calc(100vh - ${isDesktop ? "85" : "66"}px)`};
  margin-top: ${isDesktop ? "85px" : "66px"};
`;

const Content = styled.div<{ isLoading: boolean }>`
  visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};
`;

type MainPanelProps = {
  isLoading: boolean;
};

function MainPanel(
  { isLoading }: MainPanelProps,
  ref: ForwardedRef<HTMLDivElement>
) {
  return (
    <Wrapper>
      <Content id="mainPanel" ref={ref} isLoading={isLoading} />
    </Wrapper>
  );
}

export default forwardRef<HTMLDivElement, MainPanelProps>(MainPanel);
