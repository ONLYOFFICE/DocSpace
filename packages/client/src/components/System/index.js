import React from "react";
import AppLoader from "@docspace/common/components/AppLoader";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import Error520 from "client/Error520";
import Error404 from "client/Error404";

function loadComponent(scope, module) {
  return async () => {
    // Initializes the share scope. This fills it with known provided modules from this build and all remotes
    await __webpack_init_sharing__("default");
    const container = window[scope]; // or get the container somewhere else
    // Initialize the container, it may provide shared modules
    await container.init(__webpack_share_scopes__.default);
    const factory = await window[scope].get(module);
    const Module = factory();
    return Module;
  };
}

const useDynamicScript = (args) => {
  const [ready, setReady] = React.useState(false);
  const [failed, setFailed] = React.useState(false);

  React.useEffect(() => {
    if (!args.url) {
      return;
    }

    const exists = document.getElementById(args.id);

    if (exists) {
      setReady(true);
      setFailed(false);
      return;
    }

    const element = document.createElement("script");

    element.id = args.id;
    element.src = args.url;
    element.type = "text/javascript";
    element.async = true;

    setReady(false);
    setFailed(false);

    element.onload = () => {
      console.log(`Dynamic Script Loaded: ${args.url}`);
      setReady(true);
    };

    element.onerror = () => {
      console.error(`Dynamic Script Error: ${args.url}`);
      setReady(false);
      setFailed(true);
    };

    document.head.appendChild(element);

    //TODO: Comment if you don't want to remove loaded remoteEntry
    return () => {
      console.log(`Dynamic Script Removed: ${args.url}`);
      document.head.removeChild(element);
    };
  }, [args.url]);

  return {
    ready,
    failed,
  };
};

const System = (props) => {
  const { ready, failed } = useDynamicScript({
    url: props.system && props.system.url,
    id: props.system && props.system.scope,
  });

  if (!props.system) {
    console.log(`Not system specified`);
    return <Error404 />;
  }

  if (!ready) {
    console.log(`Loading dynamic script: ${props.system.url}`);
    return <AppLoader />;
  }

  if (failed) {
    console.log(`Failed to load dynamic script: ${props.system.url}`);
    return <Error520 />;
  }

  const Component = React.lazy(
    loadComponent(props.system.scope, props.system.module)
  );

  return (
    <React.Suspense fallback={<AppLoader />}>
      <ErrorBoundary>
        <Component />
      </ErrorBoundary>
    </React.Suspense>
  );
};

export default System;
