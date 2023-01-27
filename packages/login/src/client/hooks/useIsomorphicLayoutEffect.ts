import { useEffect, useLayoutEffect } from 'react';

const canUseDOM = typeof window !== 'undefined';
const useIsomorphicLayoutEffect = canUseDOM ? useLayoutEffect : useEffect;

export default useIsomorphicLayoutEffect;