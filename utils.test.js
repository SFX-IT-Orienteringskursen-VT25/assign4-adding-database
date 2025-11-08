import { describe, it, expect } from 'vitest';
import { getOrInitBucket, normalizeToNumbers, appendNumbers } from './utils.js';

describe('getOrInitBucket(store, key)', () => {
  it('initializes a new bucket when key does not exist', () => {
    const store = new Map();
    const bucket = getOrInitBucket(store, 'alpha');
    expect(bucket).toEqual({ values: [], sum: 0 });
    expect(store.get('alpha')).toEqual({ values: [], sum: 0 });
  });

  it('returns existing bucket when key exists', () => {
    const store = new Map([['k1', { values: [1, 2], sum: 3 }]]);
    const bucket = getOrInitBucket(store, 'k1');
    expect(bucket).toEqual({ values: [1, 2], sum: 3 });
  });
});

describe('normalizeToNumbers(value)', () => {
  it('accepts a single number', () => {
    expect(normalizeToNumbers(7)).toEqual([7]);
  });

  it('accepts a numeric string', () => {
    expect(normalizeToNumbers('42')).toEqual([42]);
  });

  it('accepts an array and converts numeric strings', () => {
    expect(normalizeToNumbers([1, '2', 3.5, '4.5'])).toEqual([1, 2, 3.5, 4.5]);
  });

  it('filters out NaN and non-finite numbers (e.g., Infinity)', () => {
    expect(normalizeToNumbers(['x', undefined, Infinity, -Infinity])).toEqual([]);
  });

  it('keeps negatives and floats', () => {
    expect(normalizeToNumbers(['-3', -2.5, '0.5'])).toEqual([-3, -2.5, 0.5]);
  });

  it('documents current coercion behavior for "", "   ", null, true', () => {
    expect(normalizeToNumbers(['', '   ', null, true])).toEqual([0, 0, 0, 1]);
  });
});

describe('appendNumbers(store, key, incoming)', () => {
  it('creates a new bucket and appends numbers when key is new', () => {
    const store = new Map();
    const result = appendNumbers(store, 'nums', [1, '2', 3.5]);
    expect(result.error).toBeUndefined();
    expect(result.appended).toEqual([1, 2, 3.5]);
    expect(result.bucket).toEqual({ values: [1, 2, 3.5], sum: 6.5 });
    expect(store.get('nums')).toEqual({ values: [1, 2, 3.5], sum: 6.5 });
  });

  it('appends to an existing bucket and updates the sum', () => {
    const store = new Map([['nums', { values: [1, 2], sum: 3 }]]);
    const result = appendNumbers(store, 'nums', ['4.5', 6]);
    expect(result.error).toBeUndefined();
    expect(result.appended).toEqual([4.5, 6]);
    expect(result.bucket).toEqual({ values: [1, 2, 4.5, 6], sum: 13.5 });
    expect(store.get('nums')).toEqual({ values: [1, 2, 4.5, 6], sum: 13.5 });
  });

  it('returns an error when no valid numbers are provided', () => {
    const store = new Map();
    expect(appendNumbers(store, 'nums', 'oops')).toEqual({ error: 'No valid numbers to append' });
    expect(appendNumbers(store, 'nums', [undefined, 'x'])).toEqual({ error: 'No valid numbers to append' });
  });

  it('documents that null becomes 0 and is appended (current behavior)', () => {
    const store = new Map();
    const result = appendNumbers(store, 'k', null); 
    expect(result.appended).toEqual([0]);
    expect(result.bucket).toEqual({ values: [0], sum: 0 });
  });
});
