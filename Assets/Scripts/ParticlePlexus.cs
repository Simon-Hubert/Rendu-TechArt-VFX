using UnityEngine;
using UnityEngine.Serialization;

public class ParticlePlexus : MonoBehaviour
{
    private ParticleSystem _ps;
    private ParticleSystem.Particle[] _p;

    [SerializeField] private LineRenderer _lineRendererPrefab;
    private LineRenderer[] _lineRenderers;
    [SerializeField] private int _maxLineNumber = 500;

    [SerializeField, Range(0f, 1f)]
    private float _lineWidth;
    [SerializeField] private float _lineDist = 1.0f;
    
    void Start()
    {
        DestroyAllLineRenderersIfNotNull();

        _lineRenderers = new LineRenderer[_maxLineNumber];

        for (int i = 0; i < _lineRenderers.Length; i++)
        {
            _lineRenderers[i] =
                Instantiate(_lineRendererPrefab, transform);
        }
    }
    
    void OnDestroy()
    {
        DestroyAllLineRenderersIfNotNull();
    }
    
    void LateUpdate()
    {
        _ps ??= GetComponent<ParticleSystem>();
        if (_p == null || _p.Length != _ps.main.maxParticles)
            _p = new ParticleSystem.Particle[_ps.main.maxParticles];
        int n = _ps.GetParticles(_p);
        int lineRendererCount = 0;
        for (int i = 0; i < n; i++)
        {
            ParticleSystem.Particle particleA = _p[i];
            for (int j = i + 1; j < n; j++)
            {
                ParticleSystem.Particle particleB = _p[j];
                if (Vector3.Distance(
                        particleA.position, particleB.position) < _lineDist)
                {
                    Color colourA = particleA.GetCurrentColor(GetComponent<ParticleSystem>());
                    Color colourB = particleB.GetCurrentColor(GetComponent<ParticleSystem>());
                    if (lineRendererCount < _lineRenderers.Length)
                    {
                        LineRenderer lineRenderer = 
                            _lineRenderers[lineRendererCount];
                        lineRenderer.SetPosition(0, particleA.position);
                        lineRenderer.SetPosition(1, particleB.position);
                        lineRenderer.startColor = colourA;
                        lineRenderer.endColor = colourB;
                        float sizeA = particleA.GetCurrentSize(GetComponent<ParticleSystem>());
                        float sizeB = particleB.GetCurrentSize(GetComponent<ParticleSystem>());
                        lineRenderer.startWidth = sizeA * _lineWidth;
                        lineRenderer.endWidth = sizeB * _lineWidth;
                        lineRenderer.gameObject.SetActive(true);
                        lineRendererCount++;
                    }
                }
            }
        }
        for (int i = lineRendererCount; i < _lineRenderers.Length; i++)
        {
            _lineRenderers[i].gameObject.SetActive(false);
        }
        
    }
    
    void DestroyAllLineRenderersIfNotNull()
    {
        if (_lineRenderers != null)
        {
            for (int i = 0; i < _lineRenderers.Length; i++)
            {
                if (_lineRenderers[i] != null)
                {
                    DestroyImmediate(_lineRenderers[i].gameObject);
                }
            }
        }
    }
}