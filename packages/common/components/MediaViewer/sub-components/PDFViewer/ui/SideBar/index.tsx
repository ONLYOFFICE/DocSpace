import React, { useEffect } from "react";
import styled from "styled-components";

type PanelProps = {
  isPanelOpen: boolean;
};

const SideBarContainer = styled.aside<{ isPanelOpen: boolean }>`
  height: 100vh;
  width: 100%;

  max-width: ${(props) => (props.isPanelOpen ? "306px" : "0px")};
  visibility: ${(props) => (props.isPanelOpen ? "visible" : "hidden")};
`;

function SideBar({ isPanelOpen }: PanelProps) {
  return <SideBarContainer id="viewer-thumbnail" isPanelOpen={isPanelOpen} />;
}

export default SideBar;
