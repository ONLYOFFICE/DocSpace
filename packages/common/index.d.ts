type TFuncType = (key: string) => string;

type HTMLElementEvent<T extends HTMLElement> = Event & {
    target: T;
};

interface IProvider {
    linked: boolean;
    provider: string;
    url: string;
}
type ProvidersType = IProvider[];
