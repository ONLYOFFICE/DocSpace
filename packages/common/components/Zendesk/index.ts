import React, { useEffect } from "react";

const canUseDOM = (): boolean =>
  typeof window?.document?.createElement !== "undefined";

export const ZendeskAPI: Function = (...args: string[]) => {
  if (canUseDOM() && window?.zE) {
    window?.zE?.apply(null, args);
  } else {
    console.warn("Zendesk is not initialized yet");
  }
};

interface Props {
  zendeskKey: string;
  defer?: boolean;
  onLoaded?: () => void;
  other?: any;
}

const Zendesk = ({ zendeskKey, defer, onLoaded, ...other }: Props) => {
  const onScriptLoaded = () => {
    if (typeof onLoaded === "function") {
      onLoaded();
    }
  };

  const insertScript = (zdKey: string, d?: boolean) => {
    const script = document.createElement("script");
    if (d) {
      script.defer = true;
    } else {
      script.async = true;
    }
    script.id = "ze-snippet";
    script.src = `https://static.zdassets.com/ekr/snippet.js?key=${zdKey}`;
    script.addEventListener("load", onScriptLoaded);
    document.body.appendChild(script);
  };

  useEffect(() => {
    if (canUseDOM() && !window?.zE) {
      insertScript(zendeskKey, defer);
      window.zESettings = other;
    }

    return () => {
      if (canUseDOM() && window.zE) {
        delete window.zE;
        delete window.zESettings;
      }
    };
  }, [zendeskKey, defer]);

  return null;
};

export default Zendesk;
