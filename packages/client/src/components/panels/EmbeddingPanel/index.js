import React, { useRef, useEffect } from "react";
import { inject, observer } from "mobx-react";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import { withTranslation } from "react-i18next";
import { StyledEmbeddingPanel, StyledScrollbar } from "./StyledEmbeddingPanel";
import EmbeddingBody from "./EmbeddingBody";

const EmbeddingPanelComponent = (props) => {
  const { t, link, visible, setEmbeddingPanelIsVisible } = props;

  const embeddingLink = "embeddingLinkembeddingLinkembeddingLinkembeddingLink";

  const scrollRef = useRef(null);

  const onClose = () => {
    setEmbeddingPanelIsVisible(false);
  };

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    scrollRef.current && scrollRef.current.view.focus();

    document.addEventListener("keyup", onKeyPress);

    return () => document.removeEventListener("keyup", onKeyPress);
  });

  return (
    <StyledEmbeddingPanel>
      <Backdrop
        onClick={onClose}
        visible={visible}
        isAside={true}
        zIndex={210}
      />
      <Aside className="embedding-panel" visible={visible} onClose={onClose}>
        <div className="embedding_header">
          <Heading className="hotkeys_heading">
            {t("Files:EmbeddingSettings")}
          </Heading>
        </div>
        <StyledScrollbar ref={scrollRef} stype="mediumBlack">
          <EmbeddingBody t={t} embeddingLink={link} />
        </StyledScrollbar>
      </Aside>
    </StyledEmbeddingPanel>
  );
};

export default inject(({ dialogsStore }) => {
  const { embeddingPanelIsVisible, setEmbeddingPanelIsVisible, linkParams } =
    dialogsStore;

  return {
    visible: embeddingPanelIsVisible,
    setEmbeddingPanelIsVisible,
    link: linkParams?.link?.sharedTo?.shareLink,
  };
})(
  withTranslation(["Files", "EmbeddingPanel"])(
    observer(EmbeddingPanelComponent)
  )
);
