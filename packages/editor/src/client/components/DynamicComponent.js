import React from "react";

export function loadComponent(scope, module) {
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

export const useDynamicScript = (args) => {
  const [ready, setReady] = React.useState(false);
  const [failed, setFailed] = React.useState(false);

  React.useEffect(() => {
    if (!args.url) {
      return;
    }

    const exists = document.getElementById(args.id);

    if (exists || args?.isInit) {
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
    // return () => {
    //   console.log(`Dynamic Script Removed: ${args.url}`);
    //   document.head.removeChild(element);
    // };
  }, [args.url]);

  return {
    ready,
    failed,
  };
};

const DynamicComponent = ({ system, needProxy, ...rest }) => {
  const [isInitialized, setIsInitialized] = React.useState(false);
  const [LoadedComponent, setLoadedComponent] = React.useState();

  const { ready, failed } = useDynamicScript({
    url: system && system.url,
    id: system && system.scope,
    isInit: isInitialized,
  });

  if (!system) {
    console.log(`Not system specified`);
    throw Error("Not system specified");
  }

  if (!ready) {
    console.log(`Loading dynamic script: ${system.url}`);
    return <div className={rest.className} />;
  }

  if (failed) {
    console.log(`Failed to load dynamic script: ${system.url}`);
    throw Error("failed");
  }

  if (ready && !isInitialized) {
    setIsInitialized(true);
    const Component = React.lazy(loadComponent(system.scope, system.module));

    setLoadedComponent(Component);
  }

  const Component = React.lazy(loadComponent(system.scope, system.module));

  return (
    <React.Suspense fallback={<div />}>
      {needProxy
        ? LoadedComponent && <LoadedComponent {...rest} />
        : Component && <Component {...rest} />}
    </React.Suspense>
  );
};

export default DynamicComponent;
